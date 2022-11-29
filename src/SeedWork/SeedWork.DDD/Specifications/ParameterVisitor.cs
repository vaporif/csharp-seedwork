namespace SeedWork.DDD.Specifications
{
    using System.Linq.Expressions;

    public sealed class ParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private readonly Expression _value;

        public ParameterVisitor(ParameterExpression parameter, Expression value)
        {
            _parameter = parameter;
            _value = value;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _parameter ? _value : node;
    }
}
