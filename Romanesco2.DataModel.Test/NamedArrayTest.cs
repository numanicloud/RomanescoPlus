using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Serialization;
using Romanesco.DataModel.Test.Domain;
using Romanesco.DataModel.Test.Structural;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Test;

internal class NamedArrayTest
{
    private AggregatedFactory? _aggregatedFactory;
    
    [SetUp]
    public void Setup()
    {
        var classFactory = new ClassFactory() { CommandObserver = new NullCommandObserver() };
        _aggregatedFactory = new AggregatedFactory()
        {
            ClassFactory = classFactory,
            Factories = new IModelFactory[]
            {
                new PrimitiveFactory(),
                new NamedArrayFactory(),
                new ArrayFactory(),
                new NamedClassFactory(classFactory),
            }
        };
    }

    [Test]
    public void 名前付き配列はNamedArrayModelに変換される()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NamedSubject[]));

        model.BeginAssertion()
            .NotNull()
            .Type<NamedArrayModel>();
    }

    [Test]
    public void 名前付き配列を変換するとプロトタイプの型も一致するように変換される()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NamedSubject[]));

        if (model is not NamedArrayModel array)
        {
            throw new RequiredTestMaybeFailedException();
        }

        using var children = array.BeginAssertion()
            .NotNull()
            .Sequence(x => x.Inner.Prototype.Inner.Children);

        children.Next()
            .Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual(nameof(NamedSubject.Name), x => x.Title);
    }

    [Test]
    public void 名前付き配列にシリアライズ値を読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NamedSubject[]));
        var serialized = Serialized.Array(
            Serialized.Class(Serialized.String("First").Member("Name")));

        var loaded = _aggregatedFactory!.LoadValue(model, serialized);

        if (loaded is not NamedArrayModel { Inner.Items: [ { Inner.Children: [ { Model: StringModel str } ] } ] })
        {
            throw new RequiredTestMaybeFailedException();
        }

        str.BeginAssertion()
            .NotNull()
            .AreEqual("First", x => x.Data.Value);
    }

    [Test]
    public void シリアライズ値に変換できる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NamedSubject[]));

        if (model is not NamedArrayModel array)
        {
            throw new RequiredTestMaybeFailedException();
        }

        var newItem = array.New();
        if (newItem.Inner.Children is not [{ Model: StringModel str }])
        {
            throw new RequiredTestMaybeFailedException();
        }

        str.Data.Value = "First";

        var serialized = _aggregatedFactory!.MakeData(model);

        using var items = serialized.BeginAssertion()
            .NotNull()
            .Type<SerializedArray>()
            .Sequence(x => x.Items);

        using var children = items.Next()
            .Type<SerializedClass>()
            .Sequence(x => x.Children);

        children.Next()
            .Select(x => x.Data)
            .Type<SerializedString>()
            .AreEqual("First", x => x.Value);
    }

    [Test]
    public void 生の値に変換できる()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NamedSubject[]));
        if (model is not NamedArrayModel array)
        {
            throw new RequiredTestMaybeFailedException();
        }

        var newItem = array.New();
        if (newItem.Inner.Children is not [{Model: StringModel str}])
        {
            throw new RequiredTestMaybeFailedException();
        }

        str.Data.Value = "First";

        var decoded = _aggregatedFactory!.Decode(model, typeof(NamedSubject[]));

        using var sequence = decoded.BeginAssertion()
            .NotNull()
            .Type<NamedSubject[]>()
            .Sequence(x => x);

        sequence.Next()
            .AreEqual("First", x => x.Name);
    }

    [Test]
    public void 生の値を読み込める()
    {
        var model = _aggregatedFactory!.LoadType(typeof(NamedSubject[]));
        var rawValue = new NamedSubject[]
        {
            new NamedSubject(){Name = "Hoge"}
        };

        _aggregatedFactory!.LoadRawValue(model, rawValue);

        using var items = model.BeginAssertion()
            .NotNull()
            .Type<NamedArrayModel>()
            .Sequence(x => x.Inner.Items.ToArray());

        using var children = items.Next()
            .Type<NamedClassModel>()
            .Sequence(x => x.Inner.Children);

        children.Next()
            .Select(x => x.Model)
            .Type<StringModel>()
            .AreEqual("Hoge", x => x.Data.Value);
    }

    private class NamedSubject
    {
        [EditorName]
        public string Name { get; set; }
    }
}