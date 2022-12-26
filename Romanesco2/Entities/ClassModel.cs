using System.Reactive.Linq;
using Reactive.Bindings;

namespace Romanesco.DataModel.Entities;

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
                    return "{ " + string.Join(", ", records) + " }";
                })
                .ToReadOnlyReactiveProperty("");
        }
    }

    public ClassModel()
    {
        TextOfValue = new ReactiveProperty<string>();
    }

    public IDataModel Clone(string? title)
    {
        return new ClassModel()
        {
            Title = title ?? Title,
            TypeId = TypeId,
            Children = Children.Select(x => x.Clone()).ToArray(),
        };
    }
}