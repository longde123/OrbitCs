namespace Orbit.Util.Reflect;

public static class AnnotationUtils
{
    /// <summary>
    ///     Searches the class, supertype and interfaces recursively for an annotation.
    /// </summary>
    /// <param name="clazz">The class to search on.</param>
    /// <param name="annotation">The annotation to search for.</param>
    /// <returns>The instance of the annotation or null if no instance found.</returns>
    public static TA FindAnnotationInTree<TA>(Type clazz, Type annotation) where TA : Attribute
    {
        return CrawlHierarchy(clazz, annotation) as TA;
    }

    /// <summary>
    ///     Searches the class, supertype and interfaces recursively for an annotation.
    /// </summary>
    /// <param name="clazz">The class to search on.</param>
    /// <param name="annotation">The annotation to search for.</param>
    /// <returns>True if the annotation is found, otherwise false.</returns>
    public static bool IsAnnotationInTree<TA>(Type clazz, Type annotation) where TA : Attribute
    {
        return CrawlHierarchy(clazz, annotation) != null;
    }

    private static Attribute CrawlHierarchy(Type clazz, Type annotation)
    {
        if (Attribute.GetCustomAttribute(clazz, annotation) is Attribute attribute)
        {
            return attribute;
        }

        var nestedClazz = clazz.BaseType;
        if (nestedClazz != null)
        {
            var result = CrawlHierarchy(nestedClazz, annotation);
            if (result != null)
            {
                return result;
            }
        }

        foreach (var interfaceType in clazz.GetInterfaces())
        {
            var result = CrawlHierarchy(interfaceType, annotation);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}