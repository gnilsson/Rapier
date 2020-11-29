using AutoMapper;
using AutoMapper.QueryableExtensions;
using Rapier.External.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rapier.Internal.Utility
{
    public static class QueryUtility
    {
        public static Expression CallStringContains(
                NewArrayExpression members,
                ConstantExpression value,
                int? nIterator)
        {
            var iterator = nIterator ?? 0;
            var containsLeft = Expression.Call(
                members.Expressions[iterator], MethodFactory.Contains, value);
            if (iterator >= members.Expressions.Count - 1)
                return containsLeft;
            iterator++;
            return Expression.Or(
                containsLeft, CallStringContains(
                    members, value, iterator));
        }

        public static Expression CallDateTimeCompare(
                NewArrayExpression members,
                ConstantExpression value,
                object _)
        {
            return Expression.LessThan(
                Expression.Constant(0),
                Expression.Call(
                    members.Expressions[0], MethodFactory.CompareTo, value));
        }

        public static IQueryable<TResponse> ProjectTo<TResponse, TEntity>(this IQueryable<TEntity> query,
            IConfigurationProvider config, params string[] expandMembers)
            => expandMembers == null || expandMembers.Length == 0 ?
            query.ProjectTo<TResponse>(config) :
            query.ProjectTo<TResponse>(config, null, expandMembers);


        public static IQueryable<TEntity> ApplyPaging<TEntity>(this IQueryable<TEntity> efQuery,
            IPaginateable pagination)
            => efQuery
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }
}
