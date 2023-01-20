using System.Reactive.Linq;
using Reactive.Bindings;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Entities;

public class ClassModel : IDataModel
{
    private readonly PropertyModel[] _properties = Array.Empty<PropertyModel>();

    public required TypeId TypeId { get; init; }
    public required string Title { get; init; }
    public IReadOnlyReactiveProperty<string> TextOfValue { get; private init; }
    public EntryName EntryName { get; private init; }

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
            EntryName =
                _properties.FirstOrDefault(x =>
                    x.Attributes.Any(attr => attr.Data is EditorNameAttribute)) is not { Model: StringModel nameModel }
                    ? new NullEntryName()
                    : new MutableEntryName(nameModel.Data);
        }
    }

    public ClassModel()
    {
        TextOfValue = new ReactiveProperty<string>();
        EntryName = new NullEntryName();
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
    public abstract IReadOnlyReactiveProperty<string> Name { get; }

    public abstract EntryName Clone();
}

public class NullEntryName : EntryName
{
    public override IReadOnlyReactiveProperty<string> Name { get; } =
        new ReactiveProperty<string>("{NullEntryName}");

    public override EntryName Clone() => new NullEntryName();
}

public class MutableEntryName : EntryName
{
    public override IReadOnlyReactiveProperty<string> Name { get; }

    public MutableEntryName(IReadOnlyReactiveProperty<string> name)
    {
        Name = name;
    }

    public override EntryName Clone() => new MutableEntryName(new ReactiveProperty<string>(Name.Value));
}
