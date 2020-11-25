using Rapier.Descriptive;
using Rapier.External;
using Rapier.External.Models;
using Rapier.Internal;
using Rapier.Internal.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rapier.CommandDefinitions
{
    public class Modifier<TEntity, TCommand> :
                 IModifier<TEntity, TCommand>
                 where TEntity : IEntity
                 where TCommand : ICommand
    {
        private IDictionary<string, object> _appends;
        //private readonly ConcurrentDictionary<TCommand, Func<TEntity>> _cacheCreate;
        //private readonly ConcurrentDictionary<TCommand, Action<TEntity>> _cacheUpdate;
        private readonly MemberInfo _idMember;
        private readonly MemberInfo _createdMember;
        private readonly MemberInfo _updatedMember;

        public Func<TCommand, TEntity> Create { get; }
        public Action<TEntity, TCommand> Update { get; }

        public Modifier()
        {
            Create = CreateHandle;
            Update = UpdateHandle;
            //_cacheCreate = new ConcurrentDictionary<TCommand, Func<TEntity>>();
            //_cacheUpdate = new ConcurrentDictionary<TCommand, Action<TEntity>>();
            _idMember = typeof(TEntity).GetMember(nameof(IEntity.Id))[0];
            _createdMember = typeof(TEntity).GetMember(nameof(IEntity.CreatedDate))[0];
            _updatedMember = typeof(TEntity).GetMember(nameof(IEntity.UpdatedDate))[0];
        }

        public void Append(params (string, object)[] properties)
        {
            _appends ??= new Dictionary<string, object>();
            foreach (var prop in properties)
                if (prop.Item2 != null)
                    _appends.Add(prop.Item1, prop.Item2);
        }

        // Todo: Increase perf
        private TEntity CreateHandle(TCommand command)
        {
            if (_appends != null)
                command.RequestPropertyValues.AddRange(_appends);

            var exprs = new List<MemberAssignment>();
            foreach (var propertyKeyPair in command.RequestPropertyValues)
            {
                var member = typeof(TEntity).GetMember(propertyKeyPair.Key)[0];
                if (propertyKeyPair.Value.IsEntityCollection(out var entityType))
                {
                    var foreignType = typeof(List<>).MakeGenericType(entityType);
                    var addMethod = foreignType.GetMethod(Methods.Add);
                    var foreignEntities = propertyKeyPair.Value as IEnumerable<object>;

                    var list = Expression.ListInit(
                        Expression.New(foreignType),
                        foreignEntities.Select(entity => Expression.ElementInit(
                            addMethod, Expression.Constant(entity))));

                    exprs.Add(Expression.Bind(member, list));
                }
                else
                {
                    exprs.Add(
                        Expression.Bind(member,
                        Expression.Constant(propertyKeyPair.Value)));
                }
            }

            var now = Expression.Constant(DateTime.UtcNow);
            exprs.AddRange(
                new[]
                {
                    Expression.Bind(_idMember, Expression.Constant(Guid.NewGuid())),
                    Expression.Bind(_createdMember, now),
                    Expression.Bind(_updatedMember, now)
                });

            return Expression.Lambda<Func<TEntity>>(
                Expression.MemberInit(
                    Expression.New(typeof(TEntity)), exprs)).Compile()();
        }

        private void UpdateHandle(TEntity entity, TCommand command)
        {
            if (_appends != null)
                command.RequestPropertyValues.AddRange(_appends);

            var parameter = Expression.Parameter(typeof(TEntity));
            var exprs = new List<Expression>();
            foreach (var propertyKeyPair in command.RequestPropertyValues)
                if (propertyKeyPair.Value.IsEntityCollection(out var entityType))
                {
                    var property = Expression.Property(parameter, propertyKeyPair.Key);
                    var foreignType = typeof(ICollection<>).MakeGenericType(entityType);
                    var addMethod = foreignType.GetMethod(Methods.Add);
                    var foreignEntities = propertyKeyPair.Value as IEnumerable<object>;
                    var member = typeof(TEntity).GetProperty(propertyKeyPair.Key).GetValue(entity) as IEnumerable<object>;

                    exprs.AddRange(foreignEntities
                         .Where(foreign => !member.Contains(foreign)) // better way to check?
                         .Select(uforeign => Expression.Call(
                             property, addMethod, Expression.Constant(uforeign))));
                }
                else
                {
                    exprs.Add(
                    Expression.Assign(
                        Expression.Property(parameter, propertyKeyPair.Key),
                        Expression.Constant(propertyKeyPair.Value)));
                }

            exprs.Add(
                Expression.Assign(
                    Expression.Property(parameter, _updatedMember.Name),
                    Expression.Constant(DateTime.UtcNow)));

            Expression.Lambda<Action<TEntity>>(
                Expression.Block(exprs), parameter).Compile()(entity);
        }
    }
}
