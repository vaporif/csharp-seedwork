public class CurrentUserProvider : ICurrentUserProvider
{
    public ValueTask<int> GetCurrentUserId() => ValueTask.FromResult(0);
}
