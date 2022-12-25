using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Factories;
using Romanesco2.DataModel.Test.Structural;

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

        model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var root);

        using (var members = root.Sequence(x => x.Children))
        {
            members.Next()
                .Type<ArrayModel>()
                .Empty(x => x.Items.ToArray())
                .Equal("Ints", x => x.Title)
                .Extract(out var array);

            array.Select(x => x.Prototype)
                .Type<IntModel>()
                .Equal("Prototype(Ints)", x => x.Title);
        }
    }

    private class ClassWithArray
    {
        public int[] Ints { get; set; }
    }
}