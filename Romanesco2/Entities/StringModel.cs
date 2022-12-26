namespace Romanesco2.DataModel.Entities;

internal class StringModel : PrimitiveModel<string>
{
    public StringModel() : base("")
    {
    }

    public override IDataModel Clone(string? title)
    {
        return new StringModel() { Title = title ?? Title, Data = { Value = Data.Value } };
    }
}