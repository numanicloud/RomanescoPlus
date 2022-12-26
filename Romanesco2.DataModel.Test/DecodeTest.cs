using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Test.Domain;
using Romanesco.DataModel.Test.Structural;

namespace Romanesco.DataModel.Test;

public class DecodeTest
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
    public void Intをインスタンス化できる()
    {
        var model = Model.Int("Hoge");
        model.Data.Value = 12;

        var decoded = _aggregatedFactory!.Decode(model, typeof(int));

        decoded.BeginAssertion()
            .Type<int>()
            .AreEqual(12, x => x);
    }

    [Test]
    public void クラスをインスタンス化できる()
    {
        var integer = Model.Int("Int");
        var boolean = Model.Bool("Bool");
        var line = Model.String("String");
        var real = Model.Float("Float");
        var model = Model.Class("Root",
            typeof(ComposedClass),
            integer,
            boolean,
            line,
            real);

        integer.Data.Value = 12;
        boolean.Data.Value = true;
        line.Data.Value = "Message";
        real.Data.Value = 0.5f;

        var decoded = _aggregatedFactory!.Decode(model, typeof(ComposedClass));

        decoded.BeginAssertion()
            .Type<ComposedClass>()
            .Extract(out var root);

        root.Select(x => x.Int).EqualsTo(12);
        root.Select(x => x.Bool).EqualsTo(true);
        root.Select(x => x.String).EqualsTo("Message");
        root.Select(x => x.Float).EqualsTo(0.5f);
    }

    [Test]
    public void 配列をインスタンス化できる()
    {
        var model = Model.Array("List",
            typeof(string),
            Model.String("Prototype"),
            x => x.Data.Value = "First",
            x => x.Data.Value = "Second",
            x => x.Data.Value = "Third");

        var decoded = _aggregatedFactory!.Decode(model, typeof(string[]));

        using var array = decoded.BeginAssertion()
            .Type<string[]>()
            .Sequence(x => x);

        array.Next().EqualsTo("First");
        array.Next().EqualsTo("Second");
        array.Next().EqualsTo("Third");
    }

    [Test]
    public void 二つの配列を持つクラスを正しくインスタンス化できる()
    {
        var first = Model.Array("First",
            typeof(int),
            Model.Int("Prototype(First)"),
            x => x.Data.Value = 11,
            x => x.Data.Value = 13,
            x => x.Data.Value = 17);
        var second = Model.Array("Second",
            typeof(int),
            Model.Int("Prototype(Second)"),
            x => x.Data.Value = 23,
            x => x.Data.Value = 29,
            x => x.Data.Value = 31);
        var model = Model.Class("Root",
            typeof(DoubleList),
            first,
            second);

        var decoded = _aggregatedFactory!.Decode(model, typeof(DoubleList));

        decoded.BeginAssertion()
            .Type<DoubleList>()
            .Extract(out var root);

        using var list1 = root.Sequence(x => x.First);
        list1.Next().EqualsTo(11);
        list1.Next().EqualsTo(13);
        list1.Next().EqualsTo(17);

        using var list2 = root.Sequence(x => x.Second);
        list2.Next().EqualsTo(23);
        list2.Next().EqualsTo(29);
        list2.Next().EqualsTo(31);
    }

    private class ComposedClass
    {
        public int Int { get; set; }
        public bool Bool { get; set; }
        public string String { get; set; }
        public float Float { get; set; }
    }

    private class DoubleList
    {
        public int[] First { get; set; }
        public int[] Second { get; set; }
    }
}