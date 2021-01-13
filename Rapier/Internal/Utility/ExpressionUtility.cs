using Rapier.External;
using System;
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
                        Type.EmptyTypes)))
                .Compile();
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

        public static Expression<Func<TEntity, bool>> AndAlso<TEntity>(
                Expression<Func<TEntity, bool>> expr1,
                Expression<Func<TEntity, bool>> expr2)
                where TEntity : class, IEntity
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
    }
}
