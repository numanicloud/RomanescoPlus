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
        return LoadType(RootTitle, type, this)
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

    public SerializedData MakeData(IDataModel model)
    {
        return MakeData(model, this)
            ?? throw new Exception();
    }

    public SerializedData? MakeData(IDataModel model, IModelFactory factory)
    {
        foreach (var f in Factories)
        {
            if (f.MakeData(model, factory) is { } result)
            {
                return result;
            }
        }

        if (ClassFactory.MakeData(model, factory) is { } result2)
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

    public bool LoadRawValue(IDataModel source, object rawValue)
    {
        return LoadRawValue(source, rawValue, this);
    }

    public bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader)
    {
        foreach (var factory in Factories)
        {
            if (factory.LoadRawValue(source, rawValue, loader))
            {
                return true;
            }
        }

        if (ClassFactory.LoadRawValue(source, rawValue, loader))
        {
            return true;
        }

        return false;
    }
}