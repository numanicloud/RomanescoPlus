namespace Romanesco.DataModel.Entities;

internal record TypeId
{
    public string Name { get; }
    public string MetadataName { get; }

    public TypeId(Type type)
    {
        Name = type.Name;
        MetadataName = type.AssemblyQualifiedName ?? "";
    }
}
