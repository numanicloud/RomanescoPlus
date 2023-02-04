using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

public class NamedArrayFactory : IModelFactory
{
    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (!type.IsArray || type.GetElementType() is not { } elementType)
        {
            return null;
        }

        var prototype = loader.LoadType($"Prototype({title})", elementType, loader);
        if (prototype is not NamedClassModel proto)
        {
            return null;
        }

        return new NamedArrayModel()
        {
            Title = title,
            Inner = new ModelCollection<NamedClassModel>()
            {
                ElementType = new TypeId(elementType),
                Prototype = proto
            }
        };
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target is not NamedArrayModel model
            || data is not SerializedArray serialized)
        {
            return null;
        }

        var result = new NamedArrayModel()
        {
            Title = target.Title,
            Inner = new ModelCollection<NamedClassModel>()
            {
                Prototype = model.Inner.Prototype.Clone() as NamedClassModel
                    ?? throw new Exception(),
                ElementType = model.Inner.ElementType
            }
        };

        result.Inner.Clear();
        foreach (var item in serialized.Items)
        {
            result.Inner.Add(item, loader);
        }

        return result;
    }

    public SerializedData? MakeData(IDataModel model, IModelFactory factory)
    {
        if (model is not NamedArrayModel array)
        {
            return null;
        }

        return new SerializedArray()
        {
            Items = array.Inner.Items.Select(x => factory.MakeData(x, factory))
                .FilterNull()
                .ToArray()
        };
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        if (source is not NamedArrayModel model
            || targetType.GetElementType() is not { } elementType
            || new TypeId(elementType) != model.Inner.ElementType)
        {
            return null;
        }

        var result = Array.CreateInstance(elementType, model.Inner.Items.Count);
        foreach (var (item, i) in model.Inner.Items.Select((x, i) => (x, i)))
        {
            result.SetValue(decoder.Decode(item, elementType, decoder)
                    ?? throw new Exception(),
                i);
        }

        return result;
    }

    public bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader)
    {
        if (source is not NamedArrayModel model)
        {
            return false;
        }
        
        return rawValue switch
        {
            int[] ints => Load(ints),
            bool[] bools => Load(bools),
            float[] floats => Load(floats),
            object[] objects => Load(objects),
            _ => false
        };
        

        bool Load<T>(T[] arr) where T : notnull
        {
            model.Inner.Clear();
            foreach (var item in arr)
            {
                var newItem = model.New();
                loader.LoadRawValue(newItem, item, loader);
            }
            return true;
        }
    }
}