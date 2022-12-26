using Romanesco2.DataModel.Entities;

namespace Romanesco2.DataModel.Test;

internal static class Model
{
    public static ClassModel Class(string title, Type type, params IDataModel[] children)
    {
        return new ClassModel()
        {
            Title = title,
            TypeId = new TypeId(type),
            Children = children
        };
    }

    public static ArrayModel Array<T>(string title, T prototype, params Action<T>[] setup)
        where T : IDataModel
    {
        var array = new ArrayModel()
        {
            Title = title,
            Prototype = prototype,
        };

        foreach (var action in setup)
        {
            if (array.New() is T item)
            {
                action(item);
            }
        }

        return array;
    }

    public static IntModel Int(string title) => new() { Title = title };
    public static BoolModel Bool(string title) => new() { Title = title };
    public static StringModel String(string title) => new() { Title = title };
    public static FloatModel Float(string title) => new() { Title = title };
}
