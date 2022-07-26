public interface ICommand<TCommand>
{
    ValueTask HandleAsync(TCommand command, CancellationToken ct = default);
}
