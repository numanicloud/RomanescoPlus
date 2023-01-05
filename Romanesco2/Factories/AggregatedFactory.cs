using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

public class AggregatedFactory : IModelFactory
{
    public const string RootTitle = "Root";

    public required ClassFactory ClassFactory { private get; init; }
    public required IModelFactory[] Factories { private get; init; }

    public IDataModel LoadType(Type type)
    {
        return ClassFactory.LoadType(RootTitle, type, this)
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

    public IDataModel LoadValue(IDataModel target, SerializedData data)
    {
        return LoadValue(target, data, this)
            ?? throw new Exception();
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        foreach (var factory in Factories)
        {
            if (factory.LoadValue(target, data, loader) is { } result)
            {
                return result;
            }
        }

        if (ClassFactory.LoadValue(target, data, loader) is { } result2)
        {
            return result2;
        }

        return null;
    }

    public object Decode(IDataModel source, Type targetType)
    {
        return Decode(source, targetType, this)
            ?? throw new Exception();
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        foreach (var factory in Factories)
        {
            if (factory.Decode(source, targetType, decoder) is { } result)
            {
                return result;
            }
        }

        if (ClassFactory.Decode(source, targetType, decoder) is { } result2)
        {
            return result2;
        }

        return null;
    }
}