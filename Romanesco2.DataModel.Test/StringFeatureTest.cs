using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Test.Structural;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Test;

public class StringFeatureTest
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
    public void 属性が収集される()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SimpleNamedClass));

        using var children = model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Sequence(x => x.Children);

        using var attributes = children.Next()
            .Sequence(x => x.Attributes.ToArray());

        attributes.Next();
    }

    [Test]
    public void 属性のついたStringの値がオブジェクトの名前として取得できる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SimpleNamedClass));

        if (model is not ClassModel { Children: [ { Model: StringModel str } ] })
        {
            throw FailWithTestRequirement();
        }

        str.Data.Value = "Title1";

        model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Select(x => x.EntryName)
            .Type<MutableEntryName>()
            .AreEqual("Title1", x => x.Name.Value);
    }

    [Test]
    public void 属性のついたStringが無いクラスには名前がない()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NotNamedSubject));

        model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Select(x => x.EntryName)
            .Type<NullEntryName>();
    }

    [Test]
    public void 属性のついたStringが複数ある場合は最初のプロパティが採用される()
    {
        var model = _aggregatedFactory!.LoadType(typeof(DoubleNamedSubject));
        
        if (model is not ClassModel { Children: [ { Model: StringModel str }, { Model: StringModel str2 } ] } root)
        {
            throw FailWithTestRequirement();
        }

        str.Data.Value = "Hoge";
        str2.Data.Value = "Fuga";

        root.BeginAssertion()
            .NotNull()
            .Select(x => x.EntryName)
            .Type<MutableEntryName>()
            .AreEqual("Hoge", x => x.Name.Value);
    }

    [Test]
    public void Stringでないプロパティに属性をつけても名前として扱われない()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NotStringNameSubject));

        model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Select(x => x.EntryName)
            .Type<NullEntryName>();
    }

    private static Exception FailWithTestRequirement()
    {
        return new Exception("他のテストが失敗している可能性があります");
    }

    private class SimpleNamedClass
    {
        [EditorName] public string Name { get; set; } = "";
    }

    private class NotNamedSubject
    {
        public string NotName { get; set; } = "";
    }

    private class DoubleNamedSubject
    {
        [EditorName, EditorOrder(0)] public string First { get; set; } = "";
        [EditorName, EditorOrder(1)] public string Second { get; set; } = "";
    }

    private class NotStringNameSubject
    {
        [EditorName] public int Id { get; set; } = 0;
    }
}