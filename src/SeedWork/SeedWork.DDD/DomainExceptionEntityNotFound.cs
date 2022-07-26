public class DomainExceptionEntityNotFound : DomainException
{
    public DomainExceptionEntityNotFound(string id) : base($"Not found with ID {id}")
    {
    }

    public DomainExceptionEntityNotFound(string entityName, string id) : base(
        $"{entityName} not found with ID {id}")
    {
    }

    public static DomainExceptionEntityNotFound Create<TId>(string name, TId id)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        return new(name, id.ToString()!);
    }
}
