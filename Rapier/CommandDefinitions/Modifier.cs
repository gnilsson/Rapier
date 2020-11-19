using Rapier.External;
using Rapier.External.Models;
using Rapier.Internal;
using Rapier.Internal.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rapier.CommandDefinitions
{
    public class Modifier<TEntity, TCommand> :
                 IModifier<TEntity, TCommand>
                 where TEntity : IEntity
                 where TCommand : ICommand
    {
        private IDictionary<string, object> _appends;
        private readonly ConcurrentDictionary<TCommand, Func<TEntity>> _cacheCreate;
        private readonly ConcurrentDictionary<TCommand, Action<TEntity>> _cacheUpdate;

        public Func<TCommand, TEntity> Create { get; }
        public Action<TEntity, TCommand> Update { get; }

        public Modifier()
        {
            Create = CreateHandle;
            Update = UpdateHandle;
            _cacheCreate = new ConcurrentDictionary<TCommand, Func<TEntity>>();
            _cacheUpdate = new ConcurrentDictionary<TCommand, Action<TEntity>>();
        }

        public void Append(params (string, object)[] properties)
        {
            _appends ??= new Dictionary<string, object>();
            foreach (var prop in properties)
                if (prop.Item2 != null)
                    _appends.Add(prop.Item1, prop.Item2);
        }

        private TEntity CreateHandle(TCommand command)
        {
            if (_appends != null)
                command.RequestPropertyValues.AddRange(_appends);

            var exprs = new List<MemberAssignment>();
            foreach (var propertyKeyPair in command.RequestPropertyValues)
            {
                var member = typeof(TEntity).GetMember(propertyKeyPair.Key)[0];
                var propertyType = propertyKeyPair.Value.GetType();
                if (propertyType.IsEntityCollection(out var entityType))
                {
                    var foreignType = typeof(List<>).MakeGenericType(entityType);
                    var addMethod = foreignType.GetMethod("Add");
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

            return _cacheCreate.GetOrAdd(
                command,
                ExpressionFactory.Create<TEntity>(exprs))();
        }

        private void UpdateHandle(TEntity entity, TCommand command)
        {
            if (_appends != null)
                command.RequestPropertyValues.AddRange(_appends);

            var parameter = Expression.Parameter(typeof(TEntity));
            var exprs = new List<Expression>();
            foreach (var propertyKeyPair in command.RequestPropertyValues)
            {
                var propertyType = propertyKeyPair.Value.GetType();
                if (propertyType.IsEntityCollection(out var entityType))
                {
                    var property = Expression.Property(parameter, propertyKeyPair.Key);
                    var foreignType = typeof(ICollection<>).MakeGenericType(entityType);
                    var addMethod = foreignType.GetMethod("Add");
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
            }

            _cacheUpdate.GetOrAdd(
                command,
                ExpressionFactory.Update<TEntity>(exprs, parameter))(entity);
        }
    }
}
