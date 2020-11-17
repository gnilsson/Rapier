using Rapier.External;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using System;
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
        private IDictionary<string, (object, Type)> _appends;

        private readonly MemberInfo _idMember;
        private readonly MemberInfo _createdMember;
        private readonly MemberInfo _updatedMember;

        public Modifier()
        {
            Creator = Create;
            Updater = Update;
            _idMember = typeof(TEntity).GetMember(nameof(IEntity.Id))[0];
            _createdMember = typeof(TEntity).GetMember(nameof(IEntity.CreatedDate))[0];
            _updatedMember = typeof(TEntity).GetMember(nameof(IEntity.UpdatedDate))[0];
        }
        public IModifier<TEntity, TCommand>.UpdateDelegate Updater { get; }

        public IModifier<TEntity, TCommand>.CreateDelegate Creator { get; }

        public void Append(params (string, object)[] properties)
        {
            _appends ??= new Dictionary<string, (object, Type)>();
            foreach (var prop in properties)
            {
                if (prop.Item2 != null)
                    _appends.Add(prop.Item1, (prop.Item2, typeof(object)));
            }
        }

        private TEntity Create(TCommand command)
        {
            if (_appends != null)
                command.RequestPropertyValues.AddRange(_appends);

            var exprs = new List<MemberAssignment>();
            foreach (var property in command.RequestPropertyValues)
            {
                var member = typeof(TEntity).GetMember(property.Key)[0];
                if (property.Value.Item2.IsEntity())
                {
                    var listType = typeof(List<>).MakeGenericType(property.Value.Item2);
                    var addMethod = listType.GetMethod("Add");
                    var entities = property.Value.Item1 as IEnumerable<object>;

                    var list = Expression.ListInit(
                        Expression.New(listType),
                        entities.Select(entity => Expression.ElementInit(
                            addMethod, Expression.Constant(entity))));

                    exprs.Add(Expression.Bind(member, list));
                }
                else
                {
                    exprs.Add(
                        Expression.Bind(member,
                        Expression.Constant(property.Value.Item1)));
                }
            }

            var now = Expression.Constant(DateTime.UtcNow);
            exprs.AddRange(new[] {
                Expression.Bind(_idMember, Expression.Constant(Guid.NewGuid())),
                Expression.Bind(_createdMember, now),
                Expression.Bind(_updatedMember, now)});

            return Expression.Lambda<Func<TEntity>>(
                Expression.MemberInit(
                    Expression.New(typeof(TEntity)), exprs))
                .Compile()();
        }

        private void Update(TEntity entity, TCommand command)
        {
            if (_appends != null)
                command.RequestPropertyValues.AddRange(_appends);

            var parameter = Expression.Parameter(typeof(TEntity));
            var exprs = new List<Expression>();
            foreach (var property in command.RequestPropertyValues)
                if (property.Value.Item2.IsEntity())
                {
                    var prop = Expression.Property(parameter, property.Key);
                    var type = typeof(ICollection<>).MakeGenericType(property.Value.Item2);
                    var addMethod = type.GetMethod("Add");
                    var foreignEntities = property.Value.Item1 as IEnumerable<object>;
                    var member = typeof(TEntity).GetProperty(property.Key).GetValue(entity) as IEnumerable<object>;

                    exprs.AddRange(foreignEntities
                         .Where(e => !member.Contains(e)) // better way to check?
                         .Select(ue => Expression.Call(
                             prop, addMethod, Expression.Constant(ue))));
                }
                else
                {
                    exprs.Add(
                    Expression.Assign(
                        Expression.Property(parameter, property.Key),
                        Expression.Constant(property.Value.Item1)));
                }

            Expression.Lambda<Action<TEntity>>(
                    Expression.Block(exprs), parameter)
                    .Compile()(entity);

            entity.UpdatedDate = DateTime.UtcNow;
        }
    }
}
