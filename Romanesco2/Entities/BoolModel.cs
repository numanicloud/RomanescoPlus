namespace Romanesco.DataModel.Entities;

public class BoolModel : PrimitiveModel<bool>
{
    public BoolModel() : base(default(bool).ToString())
    {
    }

    public override IDataModel Clone(string? title)
    {
        return new BoolModel()
        {
            Title = title ?? Title,
            Data = { Value = Data.Value }
        };
    }
}