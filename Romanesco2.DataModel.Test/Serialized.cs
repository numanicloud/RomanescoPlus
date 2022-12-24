using Romanesco2.DataModel.Serialization;

namespace Romanesco2.DataModel.Test;

internal static class Serialized
{
    public static SerializedClass Class(string label, params SerializedData[] children)
    {
        return new SerializedClass()
        {
            Label = label,
            Children = children
        };
    }

    public static SerializedInt Int(string label, int value)
    {
        return new SerializedInt()
        {
            Label = label,
            Value = value
        };
    }

    public static SerializedBool Bool(string label, bool value)
    {
        return new()
        {
            Label = label,
            Value = value
        };
    }

    public static SerializedString String(string label, string value)
    {
        return new SerializedString()
        {
            Label = label,
            Value = value
        };
    }

    public static SerializedFloat Float(string label, float value)
    {
        return new SerializedFloat()
        {
            Label = label,
            Value = value
        };
    }
}
