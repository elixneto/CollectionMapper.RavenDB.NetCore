namespace CollectionMapper.RavenDB.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RavenCollectionAssignedFromAttribute<T> : Attribute where T : class
{
    public Type BaseType => typeof(T);
}
