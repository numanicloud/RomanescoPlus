namespace RomanescoPlus.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class EditorNameAttribute : Attribute
{
    public string Name { get; }

    public EditorNameAttribute(string name)
    {
        Name = name;
    }
}