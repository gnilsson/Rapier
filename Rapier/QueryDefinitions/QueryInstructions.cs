using Rapier.Configuration;
using Rapier.Descriptive;
using Rapier.External;
using Rapier.Internal;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rapier.QueryDefinitions
{
    public class QueryInstructions<TEntity> where TEntity : class, IEntity
    {
        public QueryDelegate Query { get; }
        public delegate Expression<Func<TEntity, bool>> QueryDelegate(
                        IEnumerable<IParameter> parameters);

        public QueryInstructions()
        {
            Query = QueryHandle;
        }

        private Expression<Func<TEntity, bool>> QueryHandle(
                IEnumerable<IParameter> parameters)
        {
            Expression<Func<TEntity, bool>> predicate = p => true;

            foreach (var parameter in parameters)
                predicate = ExpressionUtility.AndAlso(
                    predicate, Filter(parameter));

            return predicate;
        }

        private Expression<Func<TEntity, bool>> Filter(
                IParameter parameter)
        {
            var baseProperty = Expression.Parameter(typeof(TEntity));
            var queries = InvokeApplicableCallMethod(
                GetChildrenMembers(baseProperty, parameter), parameter);
            return Expression.Lambda<Func<TEntity, bool>>(queries, baseProperty);
        }

        private List<MemberExpression> GetChildrenMembers(
                Expression baseProperty,
                IParameter parameter)
        {
            var parent = parameter.ParentNavigationProperties == null ?
                baseProperty :
                GetProperty(baseProperty, parameter.ParentNavigationProperties);
            return parameter.NavigationProperties
                .Select(child => Expression.PropertyOrField(parent, child))
                .ToList();
        }

        private Expression GetProperty(Expression parameter, string[] nodes, int iterator = 0)
        {
            var next = Expression.PropertyOrField(parameter, nodes[iterator]);
            if (iterator < nodes.Length - 1)
                GetProperty(next, nodes, iterator++);
            return next;
        }

        private Expression InvokeApplicableCallMethod(
                List<MemberExpression> members,
                IParameter parameter)
        {
            if (parameter.Method == QueryMethod.Equal)
                return Expression.Equal(members[0], Expression.Constant(parameter.Value));

            return (Expression)MethodFactory.QueryMethodContainer
                .FirstOrDefault(x => x.Key == parameter.Method).Value
                .Invoke(
                this,
                new Expression[]
                {
                    Expression.NewArrayInit(parameter.Value.GetType(), members),
                    Expression.Constant(parameter.Value),
                    null
                });
        }
    }
}
