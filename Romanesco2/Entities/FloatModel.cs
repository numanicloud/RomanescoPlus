namespace Romanesco2.DataModel.Entities;

internal class FloatModel : PrimitiveModel<float>
{
    public FloatModel() : base(default(float).ToString())
    {
    }

    public override IDataModel Clone()
    {
        return new FloatModel()
        {
            Title = Title,
            Data = { Value = Data.Value }
        };
    }
}