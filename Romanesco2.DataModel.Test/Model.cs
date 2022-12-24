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

    public static IntModel Int(string title) => new() { Title = title };
    public static BoolModel Bool(string title) => new() { Title = title };
    public static StringModel String(string title) => new() { Title = title };
    public static FloatModel Float(string title) => new() { Title = title };
}
