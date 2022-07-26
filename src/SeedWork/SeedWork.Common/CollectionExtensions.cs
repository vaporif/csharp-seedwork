public static class CollectionExtensions
{
    public static bool ContainsIgnoreCase(this ICollection<string> source, string item) => source.Contains(item, StringComparer.OrdinalIgnoreCase);

    public static bool RemoveIfExists<T>(this ICollection<T> list, Func<T, bool> predicate, bool isSingle = true)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        var items = list.Where(predicate).ToList();

        if (isSingle)
        {
            var item = items.SingleOrDefault();
            if (item != null)
            {
                list.Remove(item);
            }
        }
        else
        {
            foreach (var item in items)
            {
                list.Remove(item);
            }
        }

        return items.Any();
    }

    public static bool AddIfMissing<T>(this ICollection<T> list, T item, Func<T, bool> predicate)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        var items = list.Where(predicate).ToList();

        if (items.Any())
        {
            return false;
        }

        list.Add(item);
        return true;
    }

    public static T AddIfMissingAndReturn<T>(this ICollection<T> list, T item, Func<T, bool> predicate)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        };

        var foundItem = list.FirstOrDefault(predicate);
        if (foundItem != null)
        {
            return foundItem;
        }

        list.Add(item);
        return item;
    }
}
