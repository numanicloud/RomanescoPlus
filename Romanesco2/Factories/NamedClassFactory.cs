using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Factories;

internal class NamedClassFactory : IModelFactory
{
    private readonly ClassFactory _classFactory;

    public NamedClassFactory(ClassFactory classFactory)
    {
        _classFactory = classFactory;
    }

    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (_classFactory.LoadType(title, type, loader) is not ClassModel cm) return null;

        var nameProperty = cm.Children.FirstOrDefault(x =>
            x.Attributes.Any(attr => attr.Data is EditorNameAttribute));
        if (nameProperty?.Model is not StringModel str) return null;

        return new NamedClassModel()
        {
            Inner = cm,
            EntryName = str.Data
        };
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target is not NamedClassModel model)
            return null;
        if (loader.LoadValue(model.Inner, data, loader) is not ClassModel inner)
            return null;

        var nameProperty = inner.Children.FirstOrDefault(x =>
            x.Attributes.Any(attr => attr.Data is EditorNameAttribute));
        if (nameProperty?.Model is not StringModel str) return null;

        return new NamedClassModel()
        {
            Inner = inner,
            EntryName = str.Data,
        };
    }

    public SerializedData? MakeData(IDataModel model, IModelFactory factory)
    {
        if (model is not NamedClassModel named) return null;
        return factory.MakeData(named.Inner, factory);
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        if (source is not NamedClassModel named) return null;
        return decoder.Decode(named.Inner, targetType, decoder);
    }

    public bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader)
    {
        if (source is not NamedClassModel named) return false;

        loader.LoadRawValue(named.Inner, rawValue, loader);
        return true;
    }
}