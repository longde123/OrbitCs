namespace Orbit.Util.Misc;

public static class CollectionExtensions
{
    public static TE RandomOrNull<TE>(this ICollection<TE> collection)
    {
        try
        {
            return collection.ElementAt(new Random().Next(collection.Count));
        }
        catch (Exception e)
        {
            return default;
        }
    }
}