using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

public class EnumFactory : IModelFactory
{
    private readonly EnumMaster _master = new();

    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (!type.IsEnum) return null;

        var typeId = new TypeId(type);
        if (!_master.Map.TryGetValue(typeId, out var choices))
        {
            _master.Map[typeId] = Enum.GetValues(type).OfType<object>().ToArray();
            choices = _master.Map[typeId];
        }

        return new IntEnumModel
        {
            Type = new TypeId(type),
            Title = title,
            Choices = choices,
        };
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target is not IntEnumModel model || data is not SerializedInt serialized)
        {
            return null;
        }

        return new IntEnumModel
        {
            Title = model.Title,
            Type = model.Type,
            Choices = model.Choices,
            Selected = { Value = model.Choices.First(x => (int)x == serialized.Value) }
        };
    }

    public SerializedData? MakeData(IDataModel model, IModelFactory factory)
    {
        if (model is not IntEnumModel @enum) return null;

        return new SerializedInt()
        {
            Value = @enum.Data
        };
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        if (!targetType.IsEnum) return null;
        if (source is not IntEnumModel @enum) return null;

        foreach (var value in targetType.GetEnumValues())
        {
            if ((int)value == @enum.Data)
            {
                return value;
            }
        }
        return null;
    }

    public bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader)
    {
        if (source is not IntEnumModel model) return false;
        if (rawValue is not Enum) return false;

        model.Selected.Value = rawValue;
        return true;
    }
}