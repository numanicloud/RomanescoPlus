﻿using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Test.Domain;
using Romanesco.DataModel.Test.Structural;
using Romanesco.DataModel.Factories;

namespace Romanesco.DataModel.Test;

internal class ArrayTest
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
                new PrimitiveFactory(),
                new ArrayFactory()
            }
        };
    }

    [Test]
    public void 配列のあるクラスを読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(ClassWithArray));
        
        using var members = model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Sequence(x => x.Children);

        members.Next()
            .Select(x => x.Model)
            .Type<ArrayModel>()
            .Empty(x => x.Items.ToArray())
            .AreEqual("Ints", x => x.Title)
            .Select(x => x.Prototype)
            .Type<IntModel>()
            .AreEqual("Prototype(Ints)", x => x.Title);
    }

    [Test]
    public void 配列に要素を追加できる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(ClassWithArray));

        if (model is not ClassModel { Children: [{ Model: ArrayModel { Items: [] } array }] })
        {
            throw FailWithTestRequirement();
        }

        array.New();

        using var members = array.BeginAssertion()
            .NotNull()
            .Sequence(x => x.Items.ToArray());

        members.Next()
            .Type<IntModel>()
            .AreEqual("Item(Ints)", x => x.Title)
            .AreEqual(0, x => x.Data.Value);
    }

    [Test]
    public void 配列のInt要素の値を変更できる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(ClassWithArray));

        if (model is not ClassModel { Children: [{ Model: ArrayModel { Items: [] } array }] })
        {
            throw FailWithTestRequirement();
        }

        var item = array.New();
        if (item is not IntModel i)
        {
            throw FailWithTestRequirement();
        }

        i.Data.Value = 13;

        using var members = array.BeginAssertion()
            .NotNull()
            .Sequence(x => x.Items.ToArray());

        members.Next()
            .Type<IntModel>()
            .AreEqual("Item(Ints)", x => x.Title)
            .AreEqual(13, x => x.Data.Value);
    }

    [Test]
    public void クラスの配列を読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(ArrayWithComplex));

        using var members = model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Sequence(x => x.Children.ToArray());

        using var members2 = members.Next()
            .Select(x => x.Model)
            .Type<ArrayModel>()
            .Select(x => x.Prototype)
            .Type<ClassModel>()
            .Sequence(x => x.Children.ToArray());

        members2.Next()
            .Select(x => x.Model)
            .Type<IntModel>();

        members2.Next()
            .Select(x => x.Model)
            .Type<BoolModel>();
    }

    [Test]
    public void 配列に値を読み込める()
    {
        var model = Model.Array("List", typeof(int), Model.Int("Item(List)"));

        var serialized = Serialized.Array(
            Serialized.Int(2),
            Serialized.Int(11));

        var result = _aggregatedFactory!.LoadValue(model, serialized);

        using var items = result.BeginAssertion()
            .NotNull()
            .Type<ArrayModel>()
            .Sequence(x => x.Items.ToArray());

        items.Next()
            .Type<IntModel>()
            .AreEqual(2, x => x.Data.Value);

        items.Next()
            .Type<IntModel>()
            .AreEqual(11, x => x.Data.Value);
    }

    private static Exception FailWithTestRequirement()
    {
        return new Exception("他のテストが失敗している可能性があります");
    }

    private class ClassWithArray
    {
        public int[] Ints { get; set; }
    }

    private class ArrayWithComplex
    {
        public ComplexClass[] Objects { get; set; }
    }

    private class ComplexClass
    {
        public int X { get; set; }
        public bool Flag { get; set; }
    }
}