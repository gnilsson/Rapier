using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Rapier.Descriptive;
using Rapier.External.Models;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Rapier.Internal.Utility
{
    public static class QueryUtility
    {
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

        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(
             this IQueryable<TEntity> source,
             OrderByParameter orderParameter)
        {
            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type, "p");
            var property = Expression.Property(parameter, orderParameter.Node);

            var propertyAccess = Expression.MakeMemberAccess(parameter, property.Member);
            var orderByExpr = Expression.Lambda(propertyAccess, parameter);
            var methodName = orderParameter.SortDirection == ListSortDirection.Ascending ?
                Method.OrderBy :
                Method.OrderByDescending;

            var callExpr = Expression.Call(
                typeof(Queryable), methodName,
                new Type[] { type, property.Type },
                source.Expression, Expression.Quote(orderByExpr));
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(callExpr);
        }

        public static IQueryable<TEntity> IncludeBy<TEntity>(
            this IQueryable<TEntity> source,
            ICollection<string[]> details)
            where TEntity : class
        {
            foreach (var detail in details)
                source = source.Include(string.Join(".", detail));
            return source;
        }
    }
}
