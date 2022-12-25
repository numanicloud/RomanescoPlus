using System.Collections.ObjectModel;
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

        var reactive = Items.ToReadOnlyReactiveCollection(x => x.TextOfValue);
        TextOfValue = Observable.Merge(reactive.ObserveAddChanged().Discard(),
                reactive.ObserveMoveChanged().Discard(),
                reactive.ObserveRemoveChanged().Discard(),
                reactive.ObserveReplaceChanged().Discard(),
                reactive.ObserveResetChanged())
            .Select(_ => "[" + string.Join(", ", Items.Select(x => x.TextOfValue.Value)) + "]")
            .ToReadOnlyReactiveProperty("[]");
    }

    public void New()
    {
        _items.Add(Prototype.Clone());
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

    public IDataModel Clone()
    {
        var result = new ArrayModel()
        {
            Title = Title,
            Prototype = Prototype.Clone()
        };

        foreach (var child in Items)
        {
            result._items.Add(child.Clone());
        }

        return result;
    }
}