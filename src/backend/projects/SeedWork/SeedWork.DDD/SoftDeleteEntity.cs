namespace SeedWork.DDD;

public class SoftDeleteEntity :  ISoftDeleteEntity
{
    public bool IsDeleted { get; }

    public void SetDeleted(bool isDeleted = true) => IsDeleted = isDeleted;
}
