namespace Post.Common.Extensions;

public static class EntityExtensions
{
    public static List<T> AsSequence<T>(this T entity)
    {
        return new List<T>() { entity };
    }
}