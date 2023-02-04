using System.Reflection;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

public class ArrayFactory : IModelFactory
{
    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (!type.IsArray || type.GetElementType() is not { } elementType) return null;

        var prototype = loader.LoadType($"Prototype({title})", elementType, loader)
            ?? throw new Exception();
        var typeId = new TypeId(elementType);
        return new ArrayModel()
        {
            Title = title,
            Delegation = new ModelCollection<IDataModel>()
            {
                Prototype = prototype,
                ElementType = typeId,
            }
        };
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target is not ArrayModel model
            || data is not SerializedArray serialized) return null;

        var prototype = model.Prototype.Clone();
        var result = new ArrayModel()
        {
            Title = target.Title,
            Delegation = new ModelCollection<IDataModel>()
            {
                Prototype = prototype,
                ElementType = model.ElementType,
            },
        };

        result.Clear();
        foreach (var item in serialized.Items)
        {
            result.Add(item, loader);
        }

        return result;
    }

    public SerializedData? MakeData(IDataModel model, IModelFactory factory)
    {
        if (model is not ArrayModel array) return null;

        return new SerializedArray()
        {
            Items = array.Items.Select(x => factory.MakeData(x, factory))
                .FilterNull()
                .ToArray(),
        };
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        if (source is not ArrayModel model
            || targetType.GetElementType() is not { } elementType
            || new TypeId(elementType) != model.ElementType) return null;

        var result = Array.CreateInstance(elementType, model.Items.Count);
        foreach (var (item, i) in model.Items.Select((x, i) => (x, i)))
        {
            result.SetValue(decoder.Decode(item, elementType, decoder)
                    ?? throw new InvalidOperationException(),
                i);
        }

        return result;
    }

    public bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader)
    {
        if (source is not ArrayModel model) return false;

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
            model.Clear();
            foreach (var item in arr)
            {
                var newItem = model.New();
                loader.LoadRawValue(newItem, item, loader);
            }
            return true;
        }
    }
}