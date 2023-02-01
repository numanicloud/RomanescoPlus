using Reactive.Bindings;
using Romanesco.DataModel.Entities.Component;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Entities;

public class NamedClassModel : IDataModel
{
    public required ClassModel Inner { get; init; }
    public required IReadOnlyReactiveProperty<string> EntryName { get; init; }

    public TypeId TypeId => Inner.TypeId;
    public string Title => Inner.Title;
    public IReadOnlyReactiveProperty<string> TextOfValue => Inner.TextOfValue;

    private NamedClassModel()
    {
    }

    public IDataModel Clone(string? title = null)
    {
        // ViewにはReadOnlyReactivePropertyとしてバインドされているように見えるが、
        // Viewは動的な型を見るので書き換え可能なReactivePropertyを見る
        return Inner.Clone(title) is ClassModel cm
            && Create(cm) is { } result
                ? result
                : throw new Exception();
    }

    public static NamedClassModel? Create(ClassModel inner)
    {
        return inner.Children
            .Select(x =>
            {
                return x.Attributes.Any(attr => attr.Data is EditorNameAttribute)
                    ? x.Model
                    : null;
            })
            .FilterNull()
            .OfType<StringModel>()
            .FirstOrDefault() is not { } nameModel
                ? null
                : new NamedClassModel()
                {
                    Inner = inner,
                    EntryName = nameModel.Data
                };
    }
}

public class NamedArrayModel : IDataModel
{
    public required ArrayModel Inner { private get; init; }

    public string Title => Inner.Title;
    public IReadOnlyReactiveProperty<string> TextOfValue => Inner.TextOfValue;

    public IDataModel Clone(string? title = null)
    {
        return new NamedArrayModel()
        {
            Inner = Inner.Clone(title) as ArrayModel ?? throw new Exception(),
        };
    }
}