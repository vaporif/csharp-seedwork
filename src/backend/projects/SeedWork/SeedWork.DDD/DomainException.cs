namespace SeedWork.DDD;
public class DomainException : Exception
{
    public DomainException(string exception) : base(exception)
    {
    } 
}
