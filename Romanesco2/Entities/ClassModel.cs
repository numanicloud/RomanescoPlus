using System.Diagnostics;
using System.Reactive.Linq;
using Reactive.Bindings;
using Romanesco.DataModel.Entities.Component;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Entities;

public class ClassModel : IDataModel
{
    private readonly PropertyModel[] _properties = Array.Empty<PropertyModel>();

    public required TypeId TypeId { get; init; }
    public required string Title { get; init; }
    public EntryName EntryName { get; private init; }

    public ClassIdProvider? IdProvider { get; set; }

    public required PropertyModel[] Children
    {
        get => _properties;
        init
        {
            _properties = value;
            EntryName =
                _properties.FirstOrDefault(x =>
                    x.Attributes.Any(attr => attr.Data is EditorNameAttribute)) is not { Model: StringModel nameModel }
                    ? new NullEntryName()
                    : new MutableEntryName(nameModel.Data);
        }
    }

    public ClassModel()
    {
        EntryName = new NullEntryName();
    }

    public IDataModel Clone(string? title)
    {
        var result = new ClassModel()
        {
            Title = title ?? Title,
            TypeId = TypeId,
            Children = Children.Select(x => x.Clone()).ToArray(),
        };
        if (IdProvider is not null)
        {
            result.IdProvider = new ClassIdProvider()
            {
                PropertyName = IdProvider.PropertyName,
                Self = result
            };
        }
        return result;
    }
}

public class ClassIdProvider
{
    private IntModel? _idModel;

    public required ClassModel Self { get; init; }
    public required string PropertyName { get; init; }
    public IntModel IdModel => _idModel ??= GetIdModel();

    public IntModel GetIdModel()
    {
        Debug.WriteLine("GetIdModel");
        return Self.Children
            .FirstOrDefault(x => x.Model.Title == PropertyName)
            ?.Model as IntModel ?? throw new InvalidOperationException();
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
