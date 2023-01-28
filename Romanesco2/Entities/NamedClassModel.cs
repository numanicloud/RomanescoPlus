using Reactive.Bindings;
using Romanesco.DataModel.Entities.Component;

namespace Romanesco.DataModel.Entities;

public class NamedClassModel : IDataModel
{
    public required ClassModel Inner { get; init; }
    public required IReadOnlyReactiveProperty<string> EntryName { get; init; }

    public TypeId TypeId => Inner.TypeId;
    public string Title => Inner.Title;
    public IReadOnlyReactiveProperty<string> TextOfValue => Inner.TextOfValue;

    public IDataModel Clone(string? title = null)
    {
        // ViewにはReadOnlyReactivePropertyとしてバインドされているように見えるが、
        // Viewは動的な型を見るので書き換え可能なReactivePropertyを見る
        return new NamedClassModel()
        {
            Inner = Inner.Clone(title) as ClassModel ?? throw new Exception(),
            EntryName = new ReactiveProperty<string>(EntryName.Value)
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