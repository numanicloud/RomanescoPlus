namespace Romanesco.DataModel.Entities;

public class IntModel : PrimitiveModel<int>
{
    public IntModel() : base(default(int).ToString())
    {
    }

    public override IDataModel Clone(string? title)
    {
        return new IntModel() { Title = title ?? Title, Data = { Value = Data.Value } };
    }
}