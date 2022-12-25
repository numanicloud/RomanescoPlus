using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Factories;
using Romanesco2.DataModel.Test.Fluent;

namespace Romanesco2.DataModel.Test;

internal class ArrayTest
{
    private AggregatedFactory? _aggregatedFactory;

    [SetUp]
    public void Setup()
    {
        _aggregatedFactory = new AggregatedFactory()
        {
            ClassFactory = new ClassFactory(),
            Factories = new IModelFactory[]
            {
                new PrimitiveFactory(),
                new ArrayFactory()
            }
        };
    }

    [Test]
    public void 配列のあるクラスを読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(ClassWithArray));

        model.OnObject().NotNull().AssertType<ClassModel>(out var root);
        using (var members = root.AssertSequence(x => x.Children))
        {
            members.Next().AssertType<ArrayModel>(out var array);
            array.AssertEmpty(x => x.Items.ToArray());
            array.Equals("Ints", x => x.Title);

            array.OnObject(x => x.Prototype).AssertType<IntModel>(out var integer);
            integer.Equals("Prototype(Ints)", x => x.Title);
        }
    }

    private class ClassWithArray
    {
        public int[] Ints { get; set; }
    }
}