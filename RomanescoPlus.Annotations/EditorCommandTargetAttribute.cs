namespace RomanescoPlus.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class EditorCommandTargetAttribute : Attribute
{
    public string CommandName { get; }

    public EditorCommandTargetAttribute(string commandName)
    {
        CommandName = commandName;
    }
}