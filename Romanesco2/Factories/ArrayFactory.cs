using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Serialization;

namespace Romanesco2.DataModel.Factories;

internal class ArrayFactory : IModelFactory
{
    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (!type.IsArray || type.GetElementType() is not { } elementType) return null;

        return new ArrayModel()
        {
            Title = title,
            Prototype = loader.LoadType($"Prototype({title})", elementType, loader)
                ?? throw new Exception(),
        };
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target.Title != data.Label
            || target is not ArrayModel model
            || data is not SerializedArray serialized) return null;

        var result = new ArrayModel()
        {
            Title = target.Title,
            Prototype = model.Prototype.Clone(),
        };

        foreach (var item in serialized.Items)
        {
            result.Add(item, loader);
        }

        return result;
    }
}