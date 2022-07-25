namespace SeedWork.DDD.EF;

public interface IOnSavingEntityBehavior
{
    void OnSaving(EntityState state);
}
