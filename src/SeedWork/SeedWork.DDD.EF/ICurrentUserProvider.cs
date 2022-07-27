public interface ICurrentUserProvider
{
    ValueTask<int> GetCurrentUserId();
}
