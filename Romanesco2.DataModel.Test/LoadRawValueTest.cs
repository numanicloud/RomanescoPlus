using Romanesco.DataModel.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ClassFactory = new ClassFactory(),
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

        children.Next()
            .Select(x => x.Model)
            .Type<IntModel>()
            .AreEqual(91, x => x.Data.Value);
    }

    // TODO: 他のパターンのテスト

    private class SingleIntSubject
    {
        public int Value { get; set; }
    }
}