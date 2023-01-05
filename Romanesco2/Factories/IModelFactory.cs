using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

public interface IModelFactory
{
    IDataModel? LoadType(string title, Type type, IModelFactory loader);
    IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader);
    SerializedData? MakeData(IDataModel model, IModelFactory factory);
    object? Decode(IDataModel source, Type targetType, IModelFactory decoder);
}