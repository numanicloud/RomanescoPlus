using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Serialization;
using Romanesco.DataModel.Test.Domain;
using Romanesco.DataModel.Test.Fluent;
using Romanesco.DataModel.Test.Structural;

namespace Romanesco.DataModel.Test;

public class ClassFactoryTest
{
    private AggregatedFactory? _aggregatedFactory;

    [SetUp]
    public void Setup()
    {
        _aggregatedFactory = new AggregatedFactory()
        {
            ClassFactory = new ClassFactory() { CommandObserver = new NullCommandObserver() },
            Factories = new IModelFactory[]
            {
                new PrimitiveFactory()
            }
        };
    }

    [Test]
    public void 空のクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(EmptySubject));

        data.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children));
    }

    [Test]
    public void Intメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(IntSubject));

        data.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual(nameof(IntSubject.Value), x => x.Title);
    }

    [Test]
    public void Boolメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(BoolSubject));

        data.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<BoolModel>()
            .AreEqual(nameof(BoolSubject.Value), x => x.Title);
    }

    [Test]
    public void Stringメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(StringSubject));

        data.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual(nameof(StringSubject.Value), x => x.Title);
    }

    [Test]
    public void Floatメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(FloatSubject));

        data.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<FloatModel>()
            .AreEqual(nameof(FloatSubject.Value), x => x.Title);
    }

    [Test]
    public void FloatとStringとBoolとIntを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(ComplexSubject));

        data.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<FloatModel>()
            .AreEqual(nameof(ComplexSubject.Float), x => x.Title);
        
        sequence.Next().Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual(nameof(ComplexSubject.String), x => x.Title);
        
        sequence.Next().Select(x => x.Model)
            .Type<BoolModel>()
            .AreEqual(nameof(ComplexSubject.Bool), x => x.Title);
        
        sequence.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual(nameof(ComplexSubject.Int), x => x.Title);
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
            typeof(IntSubject),
            Model.Int("Value"));

        var data = Serialized.Class(Serialized.Int(19).Member("Value"));

        var result = _aggregatedFactory!.LoadValue(model, data, _aggregatedFactory);

        result.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual("Value", x => x.Title)
            .AreEqual(19, x => x.Data.Value);
    }

    [Test]
    public void IntとFloatとStringとBoolを持つクラスに値を読み込める()
    {
        var model = Model.Class("Root",
            typeof(ComplexSubject),
            Model.Int("Int"),
            Model.Float("Float"),
            Model.String("String"),
            Model.Bool("Bool"));

        var data = Serialized.Class(Serialized.Int(15).Member("Int"),
            Serialized.Float(0.5f).Member("Float"),
            Serialized.String("Hoge").Member("String"),
            Serialized.Bool(true).Member("Bool"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        result.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual(nameof(ComplexSubject.Int), x => x.Title)
            .AreEqual(15, x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<FloatModel>()
            .AreEqual(nameof(ComplexSubject.Float), x => x.Title)
            .AreEqual(0.5f, x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual(nameof(ComplexSubject.String), x => x.Title)
            .AreEqual("Hoge", x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<BoolModel>()
            .AreEqual(nameof(ComplexSubject.Bool), x => x.Title)
            .AreEqual(true, x => x.Data.Value);
    }
    
    [Test]
    public void 値を読み込むときメンバーの順番はモデル側が優先()
    {
        var model = Model.Class("Root",
            typeof(ComplexSubject),
            Model.Int("Int"),
            Model.Float("Float"),
            Model.String("String"),
            Model.Bool("Bool"));

        var data = Serialized.Class(Serialized.Float(0.5f).Member("Float"),
            Serialized.String("Hoge").Member("String"),
            Serialized.Int(15).Member("Int"),
            Serialized.Bool(true).Member("Bool"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        result.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual(nameof(ComplexSubject.Int), x => x.Title)
            .AreEqual(15, x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<FloatModel>()
            .AreEqual(nameof(ComplexSubject.Float), x => x.Title)
            .AreEqual(0.5f, x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual(nameof(ComplexSubject.String), x => x.Title)
            .AreEqual("Hoge", x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<BoolModel>()
            .AreEqual(nameof(ComplexSubject.Bool), x => x.Title)
            .AreEqual(true, x => x.Data.Value);
    }

    [Test]
    public void 余分な値は捨てられる()
    {
        var model = Model.Class("Root",
            typeof(ComplexSubject),
            Model.Int("Int1"));

        var data = Serialized.Class(Serialized.Int(99).Member("Int1"),
            Serialized.Int(88).Member("Int2"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        using var children = result.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Sequence(x => x.Children);

        children.Next()
            .Select(x => x.Model)
            .Type<IntModel>();
    }

    [Test]
    public void 足りないメンバーには代入しない()
    {
        var model = Model.Class("Root",
            typeof(ComplexSubject),
            Model.Int("Int"),
            Model.Bool("Bool"),
            Model.String("String"),
            Model.Float("Float"),
            Model.Int("X"));

        var data = Serialized.Class(Serialized.Int(11).Member("X"));

        var result = _aggregatedFactory!.LoadValue(model, data);

        result.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var obj);

        using var sequence = obj.Sequence(x => x.Children);

        sequence.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual("Int", x => x.Title)
            .AreEqual(default, x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<BoolModel>()
            .AreEqual("Bool", x => x.Title)
            .AreEqual(default, x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual("String", x => x.Title)
            .AreEqual("", x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<FloatModel>()
            .AreEqual("Float", x => x.Title)
            .AreEqual(default, x => x.Data.Value);

        sequence.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual("X", x => x.Title)
            .AreEqual(11, x => x.Data.Value);
    }

    [Test]
    public void 二重になっているクラスを読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SecondClass));

        model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var root);

        using var children = root.Sequence(x => x.Children);

        children.Next()
            .Select(x => x.Model)
            .Type<ClassModel>()
            .AreEqual("First", x => x.Title)
            .Extract(out var second);

        using var secondChildren = second.Sequence(x => x.Children);

        secondChildren.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual("X", x => x.Title);
    }

    [Test]
    public void Order属性の順に並ぶ()
    {
        var model = _aggregatedFactory!.LoadType(typeof(Ordered));

        model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Extract(out var root);

        using var children = root.Sequence(x => x.Children);

        children.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual("Int2", x => x.Title);

        children.Next().Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual("Int1", y => y.Title);
    }

    [Test]
    public void 自動実装でないプロパティは対象外()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NotAutoImplement));

        model.OnObject()
            .NotNull()
            .AssertType<ClassModel>(a => a
                .OnSequence(b => b.Children));
    }

    // テストデータ
    private class EmptySubject
    {
    }

    private class IntSubject
    {
        public int Value { get; set; }
    }

    private class BoolSubject
    {
        public bool Value { get; set; }
    }

    private class StringSubject
    {
        public string Value { get; set; }
    }

    private class FloatSubject
    {
        public float Value { get; set; }
    }

    private class ComplexSubject
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

    private class Ordered
    {
        [RomanescoPlus.Annotations.EditorOrder(2)]
        public int Int1 { get; set; }
        [RomanescoPlus.Annotations.EditorOrder(1)]
        public int Int2 { get; set; }
    }

    private class NotAutoImplement
    {
        public int X => 1;
    }
}