using Reactive.Bindings;
using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Entities;

public class ArrayModel : IDataModel<ArrayModel>
{
    public required ModelCollection<IDataModel> Delegation { get; init; }
    public required string Title { get; init; }
    public ReadOnlyReactiveCollection<IDataModel> Items => Delegation.Items;
    public IDataModel Prototype => Delegation.Prototype;
    public TypeId ElementType => Delegation.ElementType;


    public IDataModel New() => Delegation.New($"Item({Title})");

    public void Move(int index, int newIndex) => Delegation.Move(index, newIndex);

    public void RemoveAt(int index) => Delegation.RemoveAt(index);

    public void Add(SerializedData data, IModelFactory loader) => Delegation.Add(data, loader);

    public void Clear() => Delegation.Clear();

    public void Duplicate(int index) => Delegation.Duplicate(index);

    public ArrayModel CloneStrict(string? title = null)
    {
        var prototype = Prototype.Clone(null);
        var result = new ArrayModel()
        {
            Title = title ?? Title,
            Delegation = new ModelCollection<IDataModel>()
            {
                Prototype = prototype,
                ElementType = ElementType
            }
        };

        foreach (var child in Items)
        {
            result.Delegation.Add(child.Clone(child.Title));
        }

        return result;
    }
}