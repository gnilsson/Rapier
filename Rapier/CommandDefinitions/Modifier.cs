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
        private List<string> _discards;
        //private readonly ConcurrentDictionary<TCommand, Func<TEntity>> _cacheCreate;
        //private readonly ConcurrentDictionary<TCommand, Action<TEntity>> _cacheUpdate;
        private readonly MemberInfo _idMember;
        private readonly MemberInfo _createdMember;
        private readonly MemberInfo _updatedMember;

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

        public Func<TCommand, TEntity> Create { get; }
        public Action<TEntity, TCommand> Update { get; }

        public void Discard(params string[] propertyNames)
        {
            _discards ??= new List<string>();
            _discards.AddRange(propertyNames);
        }

        public void Append(params (string, object)[] properties)
        {
            _appends ??= new Dictionary<string, object>();
            foreach (var property in properties)
                if (property.Item2 != null)
                    _appends.Add(property.Item1, property.Item2);
        }

        // Todo: Increase perf
        private TEntity CreateHandle(TCommand command)
        {
            var propertyCollection = UpdatePropertyCollection(command.RequestPropertyValues);

            var exprs = new List<MemberAssignment>();
            foreach (var propertyKeyPair in propertyCollection)
                if (command.RequestForeignEntities.TryGetValue(propertyKeyPair.Key, out var foreignEntity))
                {
                    var foreignCollectionType = typeof(List<>).MakeGenericType(foreignEntity);
                    var addMethod = foreignCollectionType.GetMethod(Methods.Add);
                    var foreignEntities = propertyKeyPair.Value as IEnumerable<object>;

                    var list = Expression.ListInit(
                        Expression.New(foreignCollectionType),
                        foreignEntities.Select(entity => Expression.ElementInit(
                            addMethod, Expression.Constant(entity))));

                    exprs.Add(
                        Expression.Bind(typeof(TEntity).GetMember(propertyKeyPair.Key)[0], list));
                }
                else
                {
                    exprs.Add(
                        Expression.Bind(typeof(TEntity).GetMember(propertyKeyPair.Key)[0],
                        Expression.Constant(propertyKeyPair.Value)));
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
            var propertyCollection = UpdatePropertyCollection(command.RequestPropertyValues);

            var parameter = Expression.Parameter(typeof(TEntity));
            var exprs = new List<Expression>();
            foreach (var propertyKeyPair in propertyCollection)
                if (command.RequestForeignEntities.TryGetValue(propertyKeyPair.Key, out var foreignEntity))
                {
                    var property = Expression.Property(parameter, propertyKeyPair.Key);
                    var foreignType = typeof(ICollection<>).MakeGenericType(foreignEntity);
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

        private IDictionary<string, object> UpdatePropertyCollection(
                IDictionary<string, object> propertyCollection)
        {
            if (_discards != null)
                foreach (var discard in _discards)
                    propertyCollection.Remove(discard);

            if (_appends != null)
                propertyCollection.AddRange(_appends);

            return propertyCollection;
        }
    }
}
