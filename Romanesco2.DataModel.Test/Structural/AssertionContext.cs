namespace Romanesco.DataModel.Test.Structural;

internal record AssertionContext<TActual>(TActual Actual)
{
    public AssertionContext<T> Type<T>() where T : TActual
    {
        Assert.That(Actual, Is.TypeOf<T>());
        if (Actual is not T cast) throw new Exception();
        return new AssertionContext<T>(cast);
    }

    public void Extract(out AssertionContext<TActual> variable)
    {
        variable = this;
    }

    public SequenceAssertionContext<TItem> Sequence<TItem>(Func<TActual, TItem[]> selector)
    {
        return new SequenceAssertionContext<TItem>(selector(Actual));
    }

    public AssertionContext<TActual> Empty<TItem>(Func<TActual, TItem[]> selector)
    {
        Sequence(selector).Dispose();
        return this;
    }

    public AssertionContext<TActual> AreEqual<TExpected>(
        TExpected expected,
        Func<TActual, TExpected> selector)
    {
        Assert.That(selector(Actual), Is.EqualTo(expected));
        return this;
    }

    public AssertionContext<TActual> EqualsTo(TActual expected)
    {
        Assert.That(Actual, Is.EqualTo(expected));
        return this;
    }

    public AssertionContext<T> Select<T>(Func<TActual, T> selector)
    {
        return new AssertionContext<T>(selector(Actual));
    }
}