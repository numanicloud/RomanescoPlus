namespace Romanesco.DataModel.Entities.Component;

public class PropertyModel
{
    public IEnumerable<ModelAttributeData> Attributes { get; init; } = Array.Empty<ModelAttributeData>();
    public required IDataModel Model { get; init; }
    public EditorCommand[] Commands { get; init; } = Array.Empty<EditorCommand>();

    public PropertyModel Clone()
    {
        return new PropertyModel()
        {
            Attributes = Attributes.ToArray(),
            Model = Model.Clone(),
            Commands = Commands.ToArray(),
        };
    }
}