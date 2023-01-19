using System.Reactive.Linq;
using Reactive.Bindings;

namespace Romanesco.DataModel.Entities;

public class ClassModel : IDataModel
{
    private readonly PropertyModel[] _properties = Array.Empty<PropertyModel>();
    
    public required TypeId TypeId { get; init; }
    public required string Title { get; init; }
    public IReadOnlyReactiveProperty<string> TextOfValue { get; private init; }

    public required PropertyModel[] Children
    {
        get => _properties;
        init
        {
            _properties = value;
            TextOfValue = _properties.Select(x => x.Model.TextOfValue)
                .Merge()
                .Select(_ =>
                {
                    var records = _properties.Select(x => $"{x.Model.Title} = {x.Model.TextOfValue.Value}");
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

public abstract class EntryName
{
    public abstract EntryName Clone();
}

public class NullEntryName : EntryName
{
    public override EntryName Clone() => new NullEntryName();
}

public class JustEntryName : EntryName
{
    public ReactiveProperty<string> Name { get; } = new ();

    public override EntryName Clone() => new JustEntryName()
    {
        Name = { Value = Name.Value }
    };
}
