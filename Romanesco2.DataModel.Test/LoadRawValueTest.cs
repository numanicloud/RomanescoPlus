using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Test.Domain;
using Romanesco.DataModel.Test.Structural;

namespace Romanesco.DataModel.Test;

internal class LoadRawValueTest
{
    private AggregatedFactory? _aggregatedFactory;

    [SetUp]
    public void Setup()
    {
        _aggregatedFactory = new AggregatedFactory()
        {
            ClassFactory = new ClassFactory()
            {
                CommandObserver = new NullCommandObserver()
            },
            Factories = new IModelFactory[]
            {
                new PrimitiveFactory(),
                new ArrayFactory()
            }
        };
    }

    [Test]
    public void Intの値を読み込める()
    {
        var model = Model.Int("Hoge");

        _aggregatedFactory!.LoadRawValue(model, 18);

        model.BeginAssertion()
            .NotNull()
            .AreEqual(18, x => x.Data.Value);
    }

    [Test]
    public void Intを持つクラスの値を読み込める()
    {
        var model = Model.Class("Root",
            typeof(SingleIntSubject),
            Model.Int(nameof(SingleIntSubject.Value)));
        var subject = new SingleIntSubject()
        {
            Value = 91
        };

        _aggregatedFactory!.LoadRawValue(model, subject);

        using var children = model.BeginAssertion()
            .NotNull()
            .Sequence(x => x.Children);

        children.Next().DataModel().IsIntProperty(91);
    }

    [Test]
    public void 四種類のプロパティを持つクラスの値を読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(CompoundClassSubject));
        var subject = new CompoundClassSubject()
        {
            Integer = 100,
            Boolean = true,
            Float = 99.9f,
            String = "Hoge"
        };

        _aggregatedFactory!.LoadRawValue(model, subject);

        using var children = model.BeginAssertion()
            .NotNull()
            .AssertClassPropertySequence();

        children.Next().DataModel().IsIntProperty(100);
        children.Next().DataModel().IsBoolProperty(true);
        children.Next().DataModel().IsFloatProperty(99.9f);
        children.Next().DataModel().IsStringProperty("Hoge");
    }

    [Test]
    public void Stringの配列の値を読み込める()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(string[]), _aggregatedFactory);
        var subject = new string[] { "First", "Second", "Third" };

        _aggregatedFactory!.LoadRawValue(model!, subject);

        using var items = model.BeginAssertion().NotNull().AssertArrayItemSequence();
        items.Next().IsStringProperty("First");
        items.Next().IsStringProperty("Second");
        items.Next().IsStringProperty("Third");
    }

    [Test]
    public void 単純なクラスの配列の値を読み込める()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(SingleIntSubject[]), _aggregatedFactory);
        var subject = new SingleIntSubject[]
        {
            new SingleIntSubject { Value = 51 },
            new SingleIntSubject { Value = 61 },
            new SingleIntSubject { Value = 71 },
        };

        _aggregatedFactory!.LoadRawValue(model!, subject);

        using var items = model.BeginAssertion()
            .NotNull()
            .AssertArrayItemSequence();

        AssertItem(items.Next(), 51);
        AssertItem(items.Next(), 61);
        AssertItem(items.Next(), 71);

        static void AssertItem(AssertionContext<IDataModel> item, int value)
        {
            using var children = item.AssertClassPropertySequence();
            children.Next().DataModel().IsIntProperty(value);
        }
    }

    [Test]
    public void 配列を持つクラスの配列の値を読み込める()
    {
        var model =
            _aggregatedFactory!.LoadType("Array", typeof(ArrayClassSubject[]), _aggregatedFactory);
        var subject = new ArrayClassSubject[]
        {
            new ArrayClassSubject
            {
                Subjects = new SingleIntSubject[]
                {
                    new SingleIntSubject(){ Value = 11}
                }
            },
            new ArrayClassSubject
            {
                Subjects = new SingleIntSubject[]
                {
                    new SingleIntSubject(){ Value = 21},
                    new SingleIntSubject(){ Value = 31},
                }
            },
        };

        _aggregatedFactory!.LoadRawValue(model!, subject);

        using var items = model.BeginAssertion().NotNull().AssertArrayItemSequence();

        {
            using var item1 = items.Next().AssertClassPropertySequence();
            using var array1 = item1.Next().DataModel().AssertArrayItemSequence();
            using var members1 = array1.Next().AssertClassPropertySequence();
            members1.Next().DataModel().IsIntProperty(11);
        }
        {
            using var item1 = items.Next().AssertClassPropertySequence();
            using var array1 = item1.Next().DataModel().AssertArrayItemSequence();
            {
                using var members1 = array1.Next().AssertClassPropertySequence();
                members1.Next().DataModel().IsIntProperty(21);
            }
            {
                using var members2 = array1.Next().AssertClassPropertySequence();
                members2.Next().DataModel().IsIntProperty(31);
            }
        }
    }

    [Test]
    public void 配列はクリアされてから値が読み込まれる()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var data = new int[] { 1, 2, 3 };
        var subject = new int[] { 11 };

        _aggregatedFactory!.LoadRawValue(model!, data);
        _aggregatedFactory!.LoadRawValue(model!, subject);

        using var items = model.BeginAssertion().NotNull().AssertArrayItemSequence();
        items.Next().IsIntProperty(11);
    }

    [Test]
    public void Intの配列の値を読み込める()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var subject = new int[] { 1, 2 };

        _aggregatedFactory!.LoadRawValue(model!, subject);

        using var items = model.BeginAssertion().NotNull().AssertArrayItemSequence();
        items.Next().IsIntProperty(1);
        items.Next().IsIntProperty(2);
    }

    private class SingleIntSubject
    {
        public int Value { get; set; }
    }

    private class CompoundClassSubject
    {
        public int Integer { get; set; }
        public bool Boolean { get; set; }
        public float Float { get; set; }
        public string String { get; set; }
    }

    private class ArrayClassSubject
    {
        public SingleIntSubject[] Subjects { get; set; }
    }
}