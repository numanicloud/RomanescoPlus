using Romanesco2.DataModel.Entities;

namespace Romanesco2.DataModel.Factories;

internal class AggregatedFactory : IModelFactory
{
    public required ClassFactory ClassFactory { private get; init; }
    public required IModelFactory[] Factories { private get; init; }

    public IDataModel LoadType(Type type)
    {
        return ClassFactory.LoadType("Root", type, this)
            ?? throw new Exception();
    }

    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        foreach (var factory in Factories)
        {
            if (factory.LoadType(title, type, loader) is { } data)
            {
                return data;
            }
        }

        if (ClassFactory.LoadType(title, type, loader) is { } data2)
        {
            return data2;
        }

        return null;
    }
}