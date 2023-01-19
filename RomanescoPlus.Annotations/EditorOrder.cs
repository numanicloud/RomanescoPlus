namespace RomanescoPlus.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EditorOrder : Attribute
{
    public int Value { get; }

    public EditorOrder(int value)
    {
        Value = value;
    }
}