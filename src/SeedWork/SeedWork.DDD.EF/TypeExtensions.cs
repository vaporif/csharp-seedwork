public static class TypeExtensions
{
    public static object GetPropertyValue(this object source, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentOutOfRangeException(nameof(propertyName));
        }

        return source?.GetType().GetProperty(propertyName)?.GetValue(source)!;
    }
}
