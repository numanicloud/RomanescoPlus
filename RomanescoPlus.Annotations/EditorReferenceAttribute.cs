namespace RomanescoPlus.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class EditorReferenceAttribute : Attribute
{
    public string MasterName { get; }

    public EditorReferenceAttribute(string masterName)
    {
        MasterName = masterName;
    }
}