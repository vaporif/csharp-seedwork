namespace SeedWork.DDD.Specifications;

using System.Linq.Expressions;
using D3SK.NetCore.Common.Entities;
using static System.Linq.Expressions.Expression;

public class AndSpecification<T> : Specification<T>
{
    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        left = left ?? throw new ArgumentNullException(nameof(left));
        right = right ?? throw new ArgumentNullException(nameof(right));

        Expression = BuildAndExpression(left, right);
    }

    public override Expression<Func<T, bool>> Expression { get; }

    private static Expression<Func<T, bool>> BuildAndExpression(Specification<T> left, Specification<T> right)
    {
        var leftExpression = left.Expression;
        var rightExpression = right.Expression;

        var targetParameter = leftExpression.Parameters[0];
        var visitor = new ParameterVisitor(rightExpression.Parameters[0], targetParameter);
        var bodyWithReplacedParameter = visitor.Visit(rightExpression.Body);

        var andExpression = AndAlso(leftExpression.Body, bodyWithReplacedParameter);

        return Lambda<Func<T, bool>>(andExpression, targetParameter);
    }
}
