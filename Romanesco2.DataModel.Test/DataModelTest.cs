using Romanesco2.DataModel.Entities;

namespace Romanesco2.DataModel.Test;

internal class DataModelTest
{
    [Test]
    public void Intの値が更新されると文字列形式に反映される()
    {
        var model = new IntModel()
        {
            Title = "test"
        };

        Assert.That(model.TextOfValue.Value, Is.EqualTo("0"));

        model.Data.Value = 12;

        Assert.That(model.TextOfValue.Value, Is.EqualTo("12"));
    }

    [Test]
    public void 子要素が更新されるとクラスの文字列形式に反映される()
    {
        var flagModel = new BoolModel()
        {
            Title = "Flag",
            Data = { Value = true }
        };

        var model = new ClassModel()
        {
            TypeId = new TypeId(typeof(DataModelTest)),
            Children = new[] { flagModel },
            Title = "Root"
        };

        Assert.That(model.TextOfValue.Value, Is.EqualTo("DataModelTest { Flag = True }"));

        flagModel.Data.Value = false;
        
        Assert.That(model.TextOfValue.Value, Is.EqualTo("DataModelTest { Flag = False }"));
    }
}