using System.Reactive.Linq;
using Reactive.Bindings;

namespace Romanesco2.DataModel.Entities;

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

internal class ClassModel : IDataModel
{
    private readonly IDataModel[] _children = Array.Empty<IDataModel>();
    
    public required TypeId TypeId { get; init; }
    public required string Title { get; init; }
    public IReadOnlyReactiveProperty<string> TextOfValue { get; private init; }

    public required IDataModel[] Children
    {
        get => _children;
        init
        {
            _children = value;
            TextOfValue = _children.Select(x => x.TextOfValue)
                .Merge()
                .Select(_ =>
                {
                    var records = _children.Select(x => $"{x.Title} = {x.TextOfValue.Value}");
                    return TypeId.Name + " { " + string.Join(", ", records) + " }";
                })
                .ToReadOnlyReactiveProperty("");
        }
    }

    public ClassModel()
    {
        TextOfValue = new ReactiveProperty<string>();
    }
}