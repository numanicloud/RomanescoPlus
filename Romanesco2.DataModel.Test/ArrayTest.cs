using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Factories;
using Romanesco2.DataModel.Serialization;
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
        
        using var members = model.BeginAssertion()
            .NotNull()
            .Type<ClassModel>()
            .Sequence(x => x.Children);

        members.Next()
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

        if (model is not ClassModel { Children: [ArrayModel { Items: [] } array] })
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

        if (model is not ClassModel { Children: [ArrayModel { Items: [] } array] })
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
            .Type<ArrayModel>()
            .Select(x => x.Prototype)
            .Type<ClassModel>()
            .Sequence(x => x.Children.ToArray());

        members2.Next()
            .Type<IntModel>();

        members2.Next()
            .Type<BoolModel>();
    }

    [Test]
    public void クラスの配列の要素のメンバーの値を変更すると文字列形式に反映される()
    {
        var model = _aggregatedFactory!.LoadType(typeof(ArrayWithComplex));

        if (model is not ClassModel { Children: [ArrayModel { Items: [] } array] })
        {
            throw FailWithTestRequirement();
        }

        var item = array.New();
        if (item is not ClassModel { Children: [IntModel i, BoolModel b] })
        {
            throw FailWithTestRequirement();
        }

        i.Data.Value = 71;
        b.Data.Value = true;

        Assert.That(model.TextOfValue.Value,
            Is.EqualTo("""
                { Objects = [{ X = 71, Flag = True }] }
                """));
    }

    [Test]
    public void 配列に値を読み込める()
    {
        var model = Model.Array("List", Model.Int("Item(List)"));

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