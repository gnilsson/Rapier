using Microsoft.EntityFrameworkCore;
using Rapier.Descriptive;
using Rapier.Exceptions;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Rapier.Internal.Utility
{
    public static class ExpressionUtility
    {
        public delegate object EmptyConstructorDelegate();
        public static EmptyConstructorDelegate CreateEmptyConstructor(Type type)
        {
            return Expression.Lambda<EmptyConstructorDelegate>(
                Expression.New(
                    type.GetConstructor(
                        Type.EmptyTypes))).Compile();
        }

        public delegate object ConstructorDelegate(params object[] args);
        public static ConstructorDelegate CreateConstructor(Type type, params Type[] parameters)
        {
            var constructorInfo = type.GetConstructor(parameters);
            var paramExpr = Expression.Parameter(typeof(object[]));

            var constructorParameters = parameters.Select((paramType, index) =>
                Expression.Convert(
                    Expression.ArrayAccess(
                        paramExpr,
                        Expression.Constant(index)),
                    paramType)).ToArray();

            var body = Expression.New(constructorInfo, constructorParameters);
            var constructor = Expression.Lambda<ConstructorDelegate>(body, paramExpr);
            return constructor.Compile();
        }

        public delegate object GenericConstructorDelegate(params object[] args);
        public static GenericConstructorDelegate CreateGenericConstructor(Type type, Type[] genericParameters, params Type[] parameters)
        {
            var constructorInfo = type.MakeGenericType(genericParameters).GetConstructor(parameters);
            var paramExpr = Expression.Parameter(typeof(object[]));

            var constructorParameters = parameters.Select((paramType, index) =>
                Expression.Convert(
                    Expression.ArrayAccess(
                        paramExpr,
                        Expression.Constant(index)),
                    paramType)).ToArray();

            var body = Expression.New(constructorInfo, constructorParameters);
            var constructor = Expression.Lambda<GenericConstructorDelegate>(body, paramExpr);
            return constructor.Compile();
        }

        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(
             this IQueryable<TEntity> source,
             OrderByParameter orderParameter)
        {
            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type, "p");
            if (!TryGetProperty(parameter, orderParameter.Node, out var property))
                throw new BadRequestException("Invalid property description");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property.Member);
            var orderByExpr = Expression.Lambda(propertyAccess, parameter);
            var methodName = orderParameter.SortDirection == ListSortDirection.Ascending ?
                Methods.OrderBy :
                Methods.OrderByDescending;

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
        //public static IIncludableQueryable<TEntity, object> IncludeBy<TEntity>(
        //        this IQueryable<TEntity> source,
        //        ICollection<string[]> details)
        //{
        //    var type = typeof(TEntity);
        //    var body = source.Expression;
        //    foreach (var detail in details)
        //    {
        //        var parameter = Expression.Parameter(type, "px");
        //        var property = Expression.Property(parameter, detail[0]);
        //        body = CallInclude(
        //            body, parameter, property, Methods.Include, 
        //            new Type[] { type, property.Type });

        //        foreach (var then in detail.Skip(1))
        //        {
        //            var previousType = property.Type.IsEnumerableType() ?
        //                property.Type.GetGenericArguments()[0] :
        //                property.Type;

        //            parameter = Expression.Parameter(previousType, "py"); // then[0].ToString().ToLower()
        //            property = Expression.Property(parameter, then);
        //            body = CallInclude(
        //                body, parameter, property, Methods.ThenInclude,
        //                new Type[] { type, previousType, property.Type });
        //        }
        //    }
        //    return Expression.Lambda<Func<IQueryable<TEntity>,
        //        IIncludableQueryable<TEntity, object>>>(
        //        body,
        //        Expression.Parameter(typeof(IQueryable<TEntity>)))
        //        .Compile()(source);
        //}

        //private static Expression CallInclude(
        //    Expression body, ParameterExpression parameter,
        //    MemberExpression property, string methodName,
        //    Type[] types)
        //{
        //    var lambda = Expression.Lambda(
        //        Expression.MakeMemberAccess(
        //            parameter, property.Member), parameter);
        //    return Expression.Call(
        //        typeof(EntityFrameworkQueryableExtensions),
        //        methodName, types, body,
        //        Expression.Quote(lambda));
        //}

        public static bool TryGetProperty(
            ParameterExpression parameter,
            string propertyName,
            out MemberExpression property)
        {
            try
            {
                property = Expression.Property(parameter, propertyName);
                return property != null;
            }
            catch (Exception)
            {
                property = null;
                return false;
            }
        }
    }
}