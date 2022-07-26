public interface IQuery<TQuery, TResult>
{
    ValueTask<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}
