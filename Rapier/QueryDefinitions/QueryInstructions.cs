using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Rapier.Configuration;
using Rapier.Descriptive;
using Rapier.External;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rapier.QueryDefinitions
{
    public class QueryInstructions<TEntity> where TEntity : class, IEntity
    {
        private readonly MethodInfo _contains;
        private readonly MethodInfo _compareTo;
        private readonly IDictionary<string, MethodInfo> _methodContainer;
        private readonly ICollection<string[]> _includerDetails;
        public QueryDelegate Querier { get; }
        public delegate Expression<Func<TEntity, bool>> QueryDelegate(
                        IEnumerable<IParameter> parameters);

        public QueryInstructions(
            IQueryConfiguration config,
            QueryConfigurationGeneral universalConfig)
        {
            Querier = Handler;
            _includerDetails = config.IncluderDetails;
            _contains = universalConfig.Methods.Contains;
            _compareTo = universalConfig.Methods.CompareTo;

            var type = typeof(QueryInstructions<TEntity>);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            _methodContainer = new Dictionary<string, MethodInfo>
            {
                { QueryMethods.StringContains,
                    type.GetMethod(nameof(CallStringContains), flags) },
                { QueryMethods.DateTimeCompare,
                    type.GetMethod(nameof(CallDateTimeCompare), flags) }
            };
        }
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> Includer()
            => source => source.IncludeBy(_includerDetails);

        //public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Includer()
        //    => source => source.IncludeBy(_includerDetails);
        //    => source => null;
        //=> source => _includerDetails == null ?
        //   source.Include(x => x) :
        //   source.IncludeBy(_includerDetails);

        public Func<IQueryable<TEntity>,
               OrderByParameter,
               IOrderedQueryable<TEntity>>
            Orderer
            = (query, parameter) =>
            {
                return parameter == null ?
                query.OrderBy(o => true) :
                query.OrderBy(parameter);
            };

        private Expression<Func<TEntity, bool>> Handler(
                IEnumerable<IParameter> parameters)
        {
            Expression<Func<TEntity, bool>> predicate = p => true;
            foreach (var parameter in parameters)
            {
                predicate = AndAlso(
                    predicate, Filter(
                        parameter));
            }
            return predicate;
        }

        private Expression<Func<TEntity, bool>> AndAlso(
                Expression<Func<TEntity, bool>> expr1,
                Expression<Func<TEntity, bool>> expr2)
        {
            ParameterExpression param = expr1.Parameters[0];
            if (ReferenceEquals(param, expr2.Parameters[0]))
            {
                return Expression.Lambda<Func<TEntity, bool>>(
                    Expression.AndAlso(expr1.Body, expr2.Body), param);
            }
            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.AndAlso(
                    expr1.Body,
                    Expression.Invoke(expr2, param)), param);
        }

        private Expression<Func<TEntity, bool>> Filter(
                IParameter parameter)
        {
            var baseProperty = Expression.Parameter(typeof(TEntity));
            var queries =
                InvokeApplicableCallMethod(
                GetChildrenMembers(baseProperty, parameter), parameter);
            return Expression.Lambda<Func<TEntity, bool>>(
                queries, baseProperty);
        }

        private List<MemberExpression> GetChildrenMembers(
                Expression baseProperty,
                IParameter parameter)
        {
            var parent = parameter.TableReferenceParents == null ?
                baseProperty :
                GetProperty(baseProperty, parameter.TableReferenceParents);
            return parameter.TableReferenceChildren
                .Select(child => Expression.PropertyOrField(parent, child))
                .ToList();
        }

        private Expression GetProperty(
                Expression parameter,
                string[] nodes,
                int iterator = 0)
        {
            var next = Expression.PropertyOrField(
                parameter, nodes[iterator]);
            if (iterator < nodes.Length - 1)
                GetProperty(next, nodes, iterator++);
            return next;
        }

        private Expression InvokeApplicableCallMethod(
                List<MemberExpression> members,
                IParameter parameter)
        {
            return (Expression)_methodContainer
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

        private Expression CallStringContains(
                NewArrayExpression members,
                ConstantExpression value,
                int? nIterator)
        {
            var iterator = nIterator ?? 0;
            var containsLeft = Expression.Call(
                members.Expressions[iterator], _contains, value);
            if (iterator >= members.Expressions.Count - 1)
                return containsLeft;
            iterator++;
            return Expression.Or(
                containsLeft, CallStringContains(
                    members, value, iterator));
        }

        private Expression CallDateTimeCompare(
                NewArrayExpression members,
                ConstantExpression value,
                object _)
        {
            return Expression.LessThan(
                Expression.Constant(0),
                Expression.Call(
                    members.Expressions[0], _compareTo, value));
        }
    }
}
