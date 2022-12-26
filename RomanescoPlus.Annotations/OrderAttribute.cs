namespace RomanescoPlus.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OrderAttribute : Attribute
{
    public int Value { get; }

    public OrderAttribute(int value)
    {
        Value = value;
    }
}