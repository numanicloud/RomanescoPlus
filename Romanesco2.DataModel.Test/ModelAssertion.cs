using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Romanesco2.DataModel.Entities;

namespace Romanesco2.DataModel.Test;

internal class ModelAssertion
{
    public static ModelAssertion Default = new ModelAssertion();

    public void AssertEquals(IDataModel left, IDataModel right)
    {
        Assert.That(left.Title, Is.EqualTo(right.Title));

        AssertEquals<IntModel, int>(left, right, x => x.Data.Value);
        AssertEquals<BoolModel, bool>(left, right, x => x.Data.Value);
        AssertEquals<StringModel, string>(left, right, x => x.Data.Value);
        AssertEquals<FloatModel, float>(left, right, x => x.Data.Value);
        AssertClass(left, right);
        AssertArray(right, left);
    }

    public void AssertEquals<T, TValue>(IDataModel left, IDataModel right, Func<T, TValue> selector)
        where T : IDataModel
    {
        if (left is T t1 && right is T t2)
        {
            Assert.That(selector(t1), Is.EqualTo(selector(t2)));
        }
    }

    public void AssertClass(IDataModel left, IDataModel right)
    {
        if (left is not ClassModel t1 || right is not ClassModel t2) return;
        if (t1.Children.Length != t2.Children.Length) return;

        for (int i = 0; i < t1.Children.Length; i++)
        {
            AssertEquals(t1.Children[i], t2.Children[i]);
        }
    }

    public void AssertArray(IDataModel left, IDataModel right)
    {
        if (left is not ArrayModel t1 || right is not ArrayModel t2) return;
        if (t1.Items.Count != t2.Items.Count) return;

        for (int i = 0; i < t1.Items.Count; i++)
        {
            AssertEquals(t1.Items[i], t2.Items[i]);
        }
    }
}