namespace Romanesco.DataModel.Entities;

public class StringModel : PrimitiveModel<string>
{
    public StringModel() : base("")
    {
    }

    public override IDataModel Clone(string? title)
    {
        return new StringModel()
        {
            Title = title ?? Title,
            Data = { Value = Data.Value }
        };
    }
}

public class ModelAttributeData
{
    public required Attribute Data { get; init; }
}