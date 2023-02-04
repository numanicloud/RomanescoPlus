using Romanesco.DataModel.Entities.Component;

namespace Romanesco.DataModel.Entities;

public class NamedArrayModel : IDataModel
{
    public required ModelCollection<NamedClassModel> Inner { get; init; }
    public required string Title { get; init; }

    public IDataModel Clone(string? title = null)
    {
        var result = new NamedArrayModel()
        {
            Inner = new ModelCollection<NamedClassModel>()
            {
                Prototype = Inner.Prototype,
                ElementType = Inner.ElementType,
            },
            Title = Title
        };

        foreach (var item in Inner.Items)
        {
            result.Inner.Add(item.Clone() as NamedClassModel ?? throw new Exception());
        }

        return result;
    }

    public NamedClassModel New() => Inner.New($"Item({Title})");

    public void Remove(NamedClassModel item) => Inner.RemoveAt(Inner.Items.IndexOf(item));

    public void MoveUp(NamedClassModel item)
    {
        var index = Inner.Items.IndexOf(item);
        Inner.Move(index, index - 1);
    }

    public void MoveDown(NamedClassModel item)
    {
        var index = Inner.Items.IndexOf(item);
        Inner.Move(index, index + 1);
    }

    public void Duplicate(NamedClassModel item)
    {
        Inner.Duplicate(Inner.Items.IndexOf(item));
    }
}
