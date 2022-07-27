namespace SeedWork.DDD.Specifications
{
    using System.Linq.Expressions;

    public abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> Expression { get; }

        public bool IsSatisfiedBy(T entity)
        {
            var predicate = Expression.Compile();
            return predicate(entity);
        }

        public Specification<T> And(Specification<T> specification) => new AndSpecification<T>(this, specification);

        public Specification<T> AndNot(Specification<T> specification) => new AndNotSpecification<T>(this, specification);
    }
}