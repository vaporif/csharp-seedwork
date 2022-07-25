namespace SeedWork.DDD;

public class SoftDeleteEntity :  ISoftDeleteEntity
{
    public bool IsDeleted { get; private set; }

    public void SetDeleted(bool isDeleted = true) => IsDeleted = isDeleted;
}
