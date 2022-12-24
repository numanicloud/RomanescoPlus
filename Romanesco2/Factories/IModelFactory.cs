using Romanesco2.DataModel.Entities;

namespace Romanesco2.DataModel.Factories;

internal interface IModelFactory
{
    IDataModel? LoadType(string title, Type type, IModelFactory loader);
}