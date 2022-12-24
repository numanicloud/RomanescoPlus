namespace Romanesco2.DataModel.Entities;

internal class BoolModel : PrimitiveModel<bool>
{
    public BoolModel() : base(default(bool).ToString())
    {
    }

    public override IDataModel Clone()
    {
        return new BoolModel()
        {
            Title = Title,
            Data = { Value = Data.Value }
        };
    }
}