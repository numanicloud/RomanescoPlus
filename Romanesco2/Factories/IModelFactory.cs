using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

internal interface IModelFactory
{
    IDataModel? LoadType(string title, Type type, IModelFactory loader);
    IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader);
}