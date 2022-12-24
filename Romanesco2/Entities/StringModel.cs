namespace Romanesco2.DataModel.Entities;

internal class StringModel : PrimitiveModel<string>
{
    public StringModel() : base("")
    {
    }

    public override IDataModel Clone()
    {
        return new StringModel()
        {
            Title = Title,
            Data = { Value = Data.Value }
        };
    }
}