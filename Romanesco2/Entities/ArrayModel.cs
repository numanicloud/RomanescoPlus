using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Entities;

public class ArrayModel : IDataModel
{
    private readonly ReactiveCollection<IDataModel> _items = new ();

    public required string Title { get; init; }
    public IReadOnlyReactiveProperty<string> TextOfValue { get; }
    public ReadOnlyReactiveCollection<IDataModel> Items { get; }
    public required IDataModel Prototype { get; init; }
    public required TypeId ElementType { get; init; }

    public ArrayModel()
    {
        Items = _items.ToReadOnlyReactiveCollection();

        var collectionChanged = _items.ObserveAddChanged().DiscardValue()
            .Merge(_items.ObserveMoveChanged().DiscardValue())
            .Merge(_items.ObserveRemoveChanged().DiscardValue())
            .Merge(_items.ObserveReplaceChanged().DiscardValue())
            .Merge(_items.ObserveResetChanged());
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
        _items.AddOnScheduler(item);
        return item;
    }

    public void Move(int index, int newIndex)
    {
        if (newIndex >= 0 && newIndex < _items.Count)
        {
            _items.MoveOnScheduler(index, newIndex);
        }
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAtOnScheduler(index);
    }

    public void Add(SerializedData data, IModelFactory loader)
    {
        var loaded = loader.LoadValue(Prototype.Clone(), data, loader);
        if (loaded != null)
        {
            _items.AddOnScheduler(loaded);
        }
    }

    public void Clear()
    {
        _items.ClearOnScheduler();
    }

    public void Duplicate(int index)
    {
        var clone = _items[index].Clone();
        _items.InsertOnScheduler(index + 1, clone);
    }

    public IDataModel Clone(string? title = null)
    {
        var result = new ArrayModel()
        {
            Title = title ?? Title,
            Prototype = Prototype.Clone(),
            ElementType = ElementType
        };

        foreach (var child in Items)
        {
            result._items.Add(child.Clone());
        }

        return result;
    }
}