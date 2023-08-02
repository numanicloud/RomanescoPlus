using System.Reactive.Concurrency;
using Reactive.Bindings;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Entities.Component;

public class ModelCollection<T> where T : class, IDataModel
{
    private readonly ReactiveCollection<T> _items = new();

    public ReadOnlyReactiveCollection<T> Items { get; }
    public required T Prototype { get; init; }
    public required TypeId ElementType { get; init; }

    public ModelCollection()
    {
        Items = _items.ToReadOnlyReactiveCollection(Scheduler.Immediate);
    }

    public T New(string title)
    {
        if (Prototype.Clone(title) is not T item)
            throw new Exception();

        _items.Add(item);
        return item;
    }

    public void Move(int index, int newIndex)
    {
        if (newIndex >= 0 && newIndex < _items.Count)
        {
            _items.Move(index, newIndex);
        }
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Add(SerializedData data, IModelFactory loader)
    {
        if (loader.LoadValue(Prototype.Clone(null), data, loader) is T loaded)
        {
            _items.Add(loaded);
        }
    }

    public void Clear()
    {
        _items.Clear();
    }

    public void Duplicate(int index)
    {
        if (_items[index].Clone(null) is T cloned)
        {
            _items.Insert(index + 1, cloned);
        }
    }
}
