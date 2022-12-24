using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Factories;

namespace Romanesco2.DataModel.Test;

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

        FluentAssertion.OnObject(data)
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children));
    }

    [Test]
    public void Intメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(IntClass));

        FluentAssertion.OnObject(data)
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => b.AssertType<IntModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(IntClass.Value))))));
    }

    [Test]
    public void Boolメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(BoolClass));

        FluentAssertion.OnObject(data)
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => b.AssertType<BoolModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(BoolClass.Value))))));
    }

    [Test]
    public void Stringメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(StringClass));

        FluentAssertion.OnObject(data)
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => b.AssertType<StringModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(StringClass.Value))))));
    }

    [Test]
    public void Floatメンバーを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(FloatClass));

        FluentAssertion.OnObject(data)
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => b.AssertType<FloatModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(FloatClass.Value))))));
    }

    [Test]
    public void FloatとStringとBoolとIntを持つクラスの構造を作れる()
    {
        var data = _aggregatedFactory!.LoadType(typeof(ComplexClass));

        FluentAssertion.OnObject(data)
            .NotNull()
            .AssertType<ClassModel>(a => a.OnSequence(b => b.Children,
                b => b.AssertType<FloatModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(ComplexClass.Float)))),
                b => b.AssertType<StringModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(ComplexClass.String)))),
                b => b.AssertType<BoolModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(ComplexClass.Bool)))),
                b => b.AssertType<IntModel>()
                    .Do(c => Assert.That(c.Title, Is.EqualTo(nameof(ComplexClass.Int))))));
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
}