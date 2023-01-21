namespace RomanescoPlus.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class EditorMasterAttribute : Attribute
{
    public string MasterName { get; }
    public string IdPropertyName { get; }

    public EditorMasterAttribute(string masterName, string idPropertyName)
    {
        MasterName = masterName;
        IdPropertyName = idPropertyName;
    }
}