namespace Romanesco.DataModel.Entities.Component;

public record TypeId
{
    public string Name { get; }
    public string MetadataName { get; }
    public string FullName { get; }

    public TypeId(Type type)
    {
        Name = type.Name;
        MetadataName = type.AssemblyQualifiedName ?? "";
        FullName = type.FullName ?? type.Name;
    }
}
