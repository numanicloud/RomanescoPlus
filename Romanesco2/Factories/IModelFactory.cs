using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Serialization;

namespace Romanesco2.DataModel.Factories;

internal interface IModelFactory
{
    IDataModel? LoadType(string title, Type type, IModelFactory loader);
    IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader);
}