using Romanesco.DataModel.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;
using Romanesco.DataModel.Test.Domain;
using Romanesco.DataModel.Test.Structural;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Test;

public class NamedClassTest
{
    private AggregatedFactory? _aggregatedFactory;

    [SetUp]
    public void Setup()
    {
        var classFactory = new ClassFactory()
        {
            CommandObserver = new NullCommandObserver()
        };
        _aggregatedFactory = new AggregatedFactory()
        {
            ClassFactory = classFactory,
            Factories = new IModelFactory[]
            {
                new NamedClassFactory(classFactory),
                new PrimitiveFactory()
            }
        };
    }

    [Test]
    public void 決まった属性のついたプロパティを持つクラスはNamedClassとして解釈される()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SimpleNamedSubject));

        model.BeginAssertion()
            .NotNull()
            .Type<NamedClassModel>();
    }

    [Test]
    public void シリアライズ値を読み込むことができる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SimpleNamedSubject));
        var serialized = Serialized.Class(
            Serialized.String("Hoge").Member("Name"));

        var loaded = _aggregatedFactory!.LoadValue(model, serialized);

        if (loaded is not NamedClassModel { Inner.Children: [ { Model: StringModel str } ] })
        {
            throw FailWithTestRequirement();
        }

        Assert.That(str.Data.Value, Is.EqualTo("Hoge"));
    }

    [Test]
    public void シリアライズ値に変換できる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SimpleNamedSubject));
        var serialized = _aggregatedFactory.MakeData(model);

        using var children = serialized.BeginAssertion()
            .NotNull()
            .Type<SerializedClass>()
            .Sequence(x => x.Children);

        children.Next()
            .AreEqual("Name", x => x.Label)
            .Select(x => x.Data)
            .Type<SerializedString>();
    }

    [Test]
    public void デコードできる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SimpleNamedSubject));
        var decoded = _aggregatedFactory!.Decode(model, typeof(SimpleNamedSubject));

        decoded.BeginAssertion()
            .NotNull()
            .Type<SimpleNamedSubject>()
            .AreEqual("", x => x.Name);
    }

    [Test]
    public void 生の値を読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(SimpleNamedSubject));
        var rawValue = new SimpleNamedSubject()
        {
            Name = "First"
        };

        var result = _aggregatedFactory!.LoadRawValue(model, rawValue);
        Assert.That(result, Is.EqualTo(true));

        using var children = model.BeginAssertion()
            .NotNull()
            .Type<NamedClassModel>()
            .Sequence(x => x.Inner.Children);

        children.Next()
            .Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual("First", x => x.Data.Value);
    }
    
    private static Exception FailWithTestRequirement()
    {
        return new Exception("他のテストが失敗している可能性があります");
    }

    private class SimpleNamedSubject
    {
        [EditorName]
        public string Name { get; set; }
    }
}