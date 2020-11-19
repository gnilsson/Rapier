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
    }
}
