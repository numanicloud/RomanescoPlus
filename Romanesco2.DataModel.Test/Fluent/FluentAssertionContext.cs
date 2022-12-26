using NUnit.Framework.Constraints;

namespace Romanesco.DataModel.Test.Fluent;

internal class FluentAssertionContext<T>
{
    public required T Context { get; init; }

    public FluentAssertionContext<T> AssertThat<TActual>(
        Func<T, TActual> selector,
        Constraint constraint)
    {
        Assert.That(selector(Context), constraint);
        return this;
    }

    public FluentAssertionContext<T> Equals<TActual>(TActual expected, Func<T, TActual> selector)
    {
        Assert.That(selector(Context), Is.EqualTo(expected));
        return this;
    }

    public FluentAssertionContext<T> Do(Action<T> assertion)
    {
        assertion(Context);
        return this;
    }

    public FluentAssertionContext<T> AssertType<TExpected>(
        Action<FluentAssertionContext<TExpected>>? assertion = null)
    {
        Assert.That(Context, Is.TypeOf<TExpected>());

        if (assertion is not null && Context is TExpected expected)
        {
            assertion(new FluentAssertionContext<TExpected>()
            {
                Context = expected
            });
        }

        return this;
    }

    public FluentAssertionContext<T> OnObject<TNext>(
        Func<T, TNext> selector,
        Action<FluentAssertionContext<TNext>> assertion)
    {
        assertion(new FluentAssertionContext<TNext> { Context = selector(Context) });
        return this;
    }

    public FluentAssertionContext<TNext> OnObject<TNext>(Func<T, TNext> selector)
    {
        return new FluentAssertionContext<TNext>()
        {
            Context = selector(Context)
        };
    }

    public FluentAssertionContext<T> OnSequence<TNext>(
        Func<T, IEnumerable<TNext>> selector,
        params Action<FluentAssertionContext<TNext>>[] assertions)
    {
        var array = selector(Context).ToArray();
        Assert.That(array.Length, Is.EqualTo(assertions.Length));

        for (int i = 0; i < assertions.Length; i++)
        {
            assertions[i].Invoke(new FluentAssertionContext<TNext>()
            {
                Context = array[i],
            });
        }

        return this;
    }

    public void AssertType<TExpected>(out FluentAssertionContext<TExpected> variable)
    {
        Assert.That(Context, Is.TypeOf<TExpected>());
        if (Context is not TExpected cast) throw new Exception();
        variable = new FluentAssertionContext<TExpected>()
        {
            Context = cast
        };
    }

    public SequenceAssertionContext<TItem> AssertSequence<TItem>(Func<T, TItem[]> selector)
    {
        return new SequenceAssertionContext<TItem>()
        {
            Context = selector(Context)
        };
    }

    public void AssertEmpty<TItem>(Func<T, TItem[]> selector)
    {
        AssertSequence(selector).Dispose();
    }
}