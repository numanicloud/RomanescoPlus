using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;

namespace Romanesco.DataModel.Test;

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
}