using Rapier.External;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rapier.CommandDefinitions
{
    public class Modifier<TEntity, TCommand> :
                 IModifier<TEntity, TCommand>
                 where TEntity : Entity
                 where TCommand : ICommand
    {
        private IDictionary<string, (object, Type)> _appends;
        public Modifier()
        {
            Creator = Create;
            Updater = Update;
        }
        public IModifier<TEntity, TCommand>.UpdateDelegate Updater { get; }
        public IModifier<TEntity, TCommand>.CreateDelegate Creator { get; }

        public void DetailedAppend(params (string, (object, Type))[] properties)
        {
            _appends ??= new Dictionary<string, (object, Type)>();
            foreach (var prop in properties)
            {
                if (prop.Item2.Item1 != null)
                    _appends.Add(prop.Item1, prop.Item2);
            }
        }

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

            var type = typeof(TEntity);
            var exprs = new List<MemberAssignment>();
            foreach (var property in command.RequestPropertyValues)
            {
                exprs.Add(
                    Expression.Bind(type.GetMember(property.Key)[0],
                    Expression.Constant(property.Value.Item1)));
            }
            return Expression.Lambda<Func<TEntity>>(
                Expression.MemberInit(
                    Expression.New(type), exprs))
                .Compile()();
        }

        private void Update(TEntity entity, TCommand command)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var typeGroups = command.RequestPropertyValues.GroupBy(c => c.Value.Item2);
            foreach (var grouping in typeGroups)
            {
                var exprs = new List<BinaryExpression>();
                foreach (var property in grouping)
                {
                    exprs.Add(
                        Expression.Assign(
                            Expression.Property(parameter, property.Key),
                            Expression.Constant(property.Value.Item1)));
                }
                Expression.Lambda<Action<TEntity>>(
                    Expression.NewArrayInit(
                        grouping.Key, exprs), parameter)
                    .Compile()(entity);
            }
        }
    }
}
