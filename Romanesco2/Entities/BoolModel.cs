namespace Romanesco2.DataModel.Entities;

internal class BoolModel : PrimitiveModel<bool>
{
    public BoolModel() : base(default(bool).ToString())
    {
    }
}