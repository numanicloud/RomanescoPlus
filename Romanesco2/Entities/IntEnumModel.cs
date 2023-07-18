using Reactive.Bindings;
using Romanesco.DataModel.Entities.Component;

namespace Romanesco.DataModel.Entities;

public class IntEnumModel : IDataModel
{
    public required string Title { get; init; }
    public required TypeId Type { get; init; }
    public required object[] Choices { get; init; }
    public int Data { get; private set; }
    public ReactiveProperty<object?> Selected { get; } = new();

    internal IntEnumModel(object? initialValue = null)
    {
        Selected.FilterNull()
            .Subscribe(x => Data = (int)x);
        Selected.Value = initialValue;
    }

    public IDataModel Clone(string? title = null)
    {
        return new IntEnumModel(Selected.Value)
        {
            Title = title ?? Title,
            Type = Type,
            Choices = Choices
        };
    }
}

internal class EnumMaster
{
    public Dictionary<TypeId, object[]> Map { get; init; } = new();
}

internal record EnumPair(int Value, string Name);