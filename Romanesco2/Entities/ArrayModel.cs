using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Romanesco2.DataModel.Factories;
using Romanesco2.DataModel.Serialization;

namespace Romanesco2.DataModel.Entities;

internal class ArrayModel : IDataModel
{
    private readonly ObservableCollection<IDataModel> _items = new ();

    public required string Title { get; init; }
    public IReadOnlyReactiveProperty<string> TextOfValue { get; }
    public ReadOnlyObservableCollection<IDataModel> Items { get; }
    public required IDataModel Prototype { get; init; }

    public ArrayModel()
    {
        Items = new ReadOnlyObservableCollection<IDataModel>(_items);

        var collectionChanged = _items.ObserveAddChanged().DiscardValue()
            .Merge(_items.ObserveMoveChanged().DiscardValue())
            .Merge(_items.ObserveRemoveChanged().DiscardValue())
            .Merge(_items.ObserveReplaceChanged().DiscardValue())
            .Merge(_items.ObserveResetChanged())
            .Publish();
        var itemUpdated = collectionChanged
            .SelectMany(_ => Items.Select(y => y.TextOfValue).Merge())
            .DiscardValue();

        TextOfValue = collectionChanged
            .Merge(itemUpdated)
            .Select(_ => "[" + string.Join(", ", Items.Select(x => x.TextOfValue.Value)) + "]")
            .ToReadOnlyReactiveProperty("[]");
    }

    public IDataModel New()
    {
        var item = Prototype.Clone($"Item({Title})");
        _items.Add(item);
        return item;
    }

    public void Move(int index, int newIndex)
    {
        _items.Move(index, newIndex);
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    public void Add(SerializedData data, IModelFactory loader)
    {
        var loaded = loader.LoadValue(Prototype.Clone(), data, loader);
        if (loaded != null)
        {
            _items.Add(loaded);
        }
    }

    public IDataModel Clone(string? title = null)
    {
        var result = new ArrayModel()
        {
            Title = title ?? Title,
            Prototype = Prototype.Clone()
        };

        foreach (var child in Items)
        {
            result._items.Add(child.Clone());
        }

        return result;
    }
}