namespace Romanesco2.DataModel.Entities;

internal class IntModel : PrimitiveModel<int>
{
    public IntModel() : base(default(int).ToString())
    {
    }

    public override IDataModel Clone()
    {
        return new IntModel()
        {
            Title = Title,
            Data = { Value = Data.Value }
        };
    }
}