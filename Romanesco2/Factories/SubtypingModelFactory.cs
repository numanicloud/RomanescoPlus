using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

internal class SubtypingModelFactory : IModelFactory
{
    public required SubtypingContext Context { get; init; }

    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        return Context.LoadUnionType(type, loader) is {} subtypes ? new SubtypingModel()
        {
            Context = Context,
            Title = title,
            Choices = subtypes
        } : null;
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        throw new NotImplementedException();
    }

    public SerializedData? MakeData(IDataModel model, IModelFactory factory)
    {
        throw new NotImplementedException();
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        throw new NotImplementedException();
    }

    public bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader)
    {
        throw new NotImplementedException();
    }
}