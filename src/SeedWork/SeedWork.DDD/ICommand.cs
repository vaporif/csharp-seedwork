public interface ICommand<TCommand>
{
    ValueTask HandleAsync(TCommand input, CancellationToken ct = default);
}
