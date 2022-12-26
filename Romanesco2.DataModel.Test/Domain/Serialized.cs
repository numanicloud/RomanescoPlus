using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Test.Domain;

internal static class Serialized
{
    public static SerializedClass Class(params SerializedMember[] children)
    {
        return new SerializedClass()
        {
            Children = children
        };
    }

    public static SerializedMember Member(this SerializedData data, string label)
    {
        return new SerializedMember()
        {
            Data = data,
            Label = label
        };
    }

    public static SerializedArray Array(params SerializedData[] items)
    {
        return new SerializedArray()
        {
            Items = items
        };
    }

    public static SerializedInt Int(int value)
    {
        return new SerializedInt()
        {
            Value = value
        };
    }

    public static SerializedBool Bool(bool value)
    {
        return new()
        {
            Value = value
        };
    }

    public static SerializedString String(string value)
    {
        return new SerializedString()
        {
            Value = value
        };
    }

    public static SerializedFloat Float(float value)
    {
        return new SerializedFloat()
        {
            Value = value
        };
    }
}
