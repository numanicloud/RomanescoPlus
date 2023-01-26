using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Test.Structural;

namespace Romanesco.DataModel.Test.Domain;

internal static class StructuralHelper
{
    public static AssertionContext<IDataModel> DataModel(this AssertionContext<PropertyModel> subject)
    {
        return subject.Select(x => x.Model);
    }

    public static void IsIntProperty(this AssertionContext<IDataModel> subject, int value)
    {
        subject.Type<IntModel>()
            .AreEqual(value, x => x.Data.Value);
    }

    public static void IsBoolProperty(this AssertionContext<IDataModel> subject, bool value)
    {
        subject.Type<BoolModel>()
            .AreEqual(value, x => x.Data.Value);
    }

    public static void IsFloatProperty(this AssertionContext<IDataModel> subject, float value)
    {
        subject.Type<FloatModel>()
            .AreEqual(value, x => x.Data.Value);
    }

    public static void IsStringProperty(this AssertionContext<IDataModel> subject, string value)
    {
        subject.Type<StringModel>()
            .AreEqual(value, x => x.Data.Value);
    }

    public static SequenceAssertionContext<PropertyModel> AssertClassPropertySequence(
        this AssertionContext<IDataModel> subject)
    {
        return subject.Type<ClassModel>()
            .Sequence(x => x.Children);
    }

    public static SequenceAssertionContext<IDataModel> AssertArrayItemSequence(
        this AssertionContext<IDataModel> subject)
    {
        return subject.Type<ArrayModel>()
            .Sequence(x => x.Items.ToArray());
    }
}