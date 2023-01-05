namespace Romanesco.DataModel.Entities;

public class FloatModel : PrimitiveModel<float>
{
    public FloatModel() : base(default(float).ToString())
    {
    }

    public override IDataModel Clone(string? title)
    {
        return new FloatModel()
        {
            Title = title ?? Title,
            Data = { Value = Data.Value }
        };
    }
}