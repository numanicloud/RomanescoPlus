using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Serialization;
using Romanesco.DataModel.Test.Domain;
using Romanesco.DataModel.Test.Fluent;

namespace Romanesco.DataModel.Test;

public class ClassFactoryTest
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
                new PrimitiveFactory()
            }
        };
    }

    [Test]
    public void 空のクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(EmptyClass));

        data.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children));
    }

    [Test]
    public void Intメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(IntClass));

        data.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => AssertModel<IntModel>(b, nameof(IntClass.Value))));
    }

    [Test]
    public void Boolメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(BoolClass));

        data.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => AssertModel<BoolModel>(b, nameof(BoolClass.Value))));
    }

    [Test]
    public void Stringメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(StringClass));

        data.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => AssertModel<StringModel>(b, nameof(StringClass.Value))));
    }

    [Test]
    public void Floatメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(FloatClass));

        data.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => AssertModel<FloatModel>(b, nameof(FloatClass.Value))));
    }

    [Test]
    public void FloatとStringとBoolとIntを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(ComplexClass));

        data.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => AssertModel<FloatModel>(b, nameof(ComplexClass.Float)),
                b => AssertModel<StringModel>(b, nameof(ComplexClass.String)),
                b => AssertModel<BoolModel>(b, nameof(ComplexClass.Bool)),
                b => AssertModel<IntModel>(b, nameof(ComplexClass.Int))));
    }

    [Test]
    public void Intに値を読み込める()
    {
        var model = new IntModel()
        {
            Title = "Data",
        };
        var data = new SerializedInt()
        {
            Value = 18
        };

        var result = _aggregatedFactory!.LoadValue(model, data, _aggregatedFactory);

        result.OnObject()
            .NotNull()
            .AssertType<IntModel>(a => a
                .Do(b => Assert.That(b.Title, Is.EqualTo("Data")))
                .Do(b => Assert.That(b.Data.Value, Is.EqualTo(18))));
    }

    [Test]
    public void Intを持つクラスに値を読み込める()
    {
        var model = Model.Class("Root",
            typeof(IntClass),
            Model.Int("Value"));

        var data = Serialized.Class(Serialized.Int(19).Member("Value"));

        var result = _aggregatedFactory!.LoadValue(model, data, _aggregatedFactory);

        result.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => AssertValue<IntModel, int>(b, "Value", x => x.Data.Value, 19)));
    }

    [Test]
    public void IntとFloatとStringとBoolを持つクラスに値を読み込める()
    {
        var model = Model.Class("Root",
            typeof(ComplexClass),
            Model.Int("Int"),
            Model.Float("Float"),
            Model.String("String"),
            Model.Bool("Bool"));

        var data = Serialized.Class(Serialized.Int(15).Member("Int"),
            Serialized.Float(0.5f).Member("Float"),
            Serialized.String("Hoge").Member("String"),
            Serialized.Bool(true).Member("Bool"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        result.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a
                .OnSequence(b => b.Children,
                    b => AssertValue<IntModel, int>(b, "Int", x => x.Data.Value, 15),
                    b => AssertValue<FloatModel, float>(b, "Float", x => x.Data.Value, 0.5f),
                    b => AssertValue<StringModel, string>(b, "String", x => x.Data.Value, "Hoge"),
                    b => AssertValue<BoolModel, bool>(b, "Bool", x => x.Data.Value, true)));
    }
    
    [Test]
    public void 値を読み込むときメンバーの順番はモデル側が優先()
    {
        var model = Model.Class("Root",
            typeof(ComplexClass),
            Model.Int("Int"),
            Model.Float("Float"),
            Model.String("String"),
            Model.Bool("Bool"));

        var data = Serialized.Class(Serialized.Float(0.5f).Member("Float"),
            Serialized.String("Hoge").Member("String"),
            Serialized.Int(15).Member("Int"),
            Serialized.Bool(true).Member("Bool"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        result.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a
                .OnSequence(b => b.Children,
                    b => AssertValue<IntModel, int>(b, "Int", x => x.Data.Value, 15),
                    b => AssertValue<FloatModel, float>(b, "Float", x => x.Data.Value, 0.5f),
                    b => AssertValue<StringModel, string>(b, "String", x => x.Data.Value, "Hoge"),
                    b => AssertValue<BoolModel, bool>(b, "Bool", x => x.Data.Value, true)));
    }

    [Test]
    public void 余分な値は捨てられる()
    {
        var model = Model.Class("Root",
            typeof(ComplexClass),
            Model.Int("Int1"));

        var data = Serialized.Class(Serialized.Int(99).Member("Int1"),
            Serialized.Int(88).Member("Int2"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        result.OnObject().NotNull().AssertType<ClassModel>(a => a
            .OnSequence(b => b.Children,
                b => b.AssertType<IntModel>()));
    }

    [Test]
    public void 足りないメンバーには代入しない()
    {
        var model = Model.Class("Root",
            typeof(ComplexClass),
            Model.Int("Int"),
            Model.Bool("Bool"),
            Model.String("String"),
            Model.Float("Float"),
            Model.Int("X"));

        var data = Serialized.Class(Serialized.Int(11).Member("X"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        result.OnObject().NotNull().AssertType<ClassModel>(a => a
            .OnSequence(b => b.Children,
                b => AssertValue<IntModel, int>(b, "Int", x => x.Data.Value, default),
                b => AssertValue<BoolModel, bool>(b, "Bool", x => x.Data.Value, default),
                b => AssertValue<StringModel, string>(b, "String", x => x.Data.Value, ""),
                b => AssertValue<FloatModel, float>(b, "Float", x => x.Data.Value, default),
                b => AssertValue<IntModel, int>(b, "X", x => x.Data.Value, 11)));
    }

    [Test]
    public void 二重になっているクラスを読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SecondClass));

        model.OnObject().NotNull().AssertType<ClassModel>(a => a
            .OnSequence(b => b.Children,
                b => b.AssertType<ClassModel>(c => c
                    .OnSequence(d => d.Children,
                        d => AssertModel<IntModel>(d, "X"))
                    .Do(d => Assert.That(d.Title, Is.EqualTo("First"))))));
    }

    private static void AssertModel<T>(FluentAssertionContext<IDataModel> data, string title) where T : IDataModel
    {
        data.AssertType<T>(a => Assert.That(a.Context.Title, Is.EqualTo(title)));
    }

    private static void AssertValue<T, TValue>(
        FluentAssertionContext<IDataModel> data,
        string title,
        Func<T, TValue> selector,
        TValue expected)
        where T : IDataModel
    {
        data.AssertType<T>(a => a
            .Do(b => Assert.That(b.Title, Is.EqualTo(title)))
            .Do(b => Assert.That(selector(b), Is.EqualTo(expected))));
    }

    private class EmptyClass
    {
    }

    private class IntClass
    {
        public int Value { get; set; }
    }

    private class BoolClass
    {
        public bool Value { get; set; }
    }

    private class StringClass
    {
        public string Value { get; set; }
    }

    private class FloatClass
    {
        public float Value { get; set; }
    }

    private class ComplexClass
    {
        public float Float { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public int Int { get; set; }
    }

    private class FirstClass
    {
        public int X { get; set; }
    }

    private class SecondClass
    {
        public FirstClass First { get; set; }
    }
}