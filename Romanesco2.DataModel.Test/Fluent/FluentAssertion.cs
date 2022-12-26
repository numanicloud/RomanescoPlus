namespace Romanesco.DataModel.Test.Fluent;

internal static class FluentAssertion
{
    public static FluentAssertionContext<T?> OnObject<T>(this T? context)
        where T : class
    {
        return new FluentAssertionContext<T?> { Context = context };
    }

    public static FluentAssertionContext<T> NotNull<T>(this FluentAssertionContext<T?> context)
        where T : class
    {
        Assert.That(context.Context, Is.Not.Null);
        if (context.Context is not { } value) throw new Exception();

        return new FluentAssertionContext<T> { Context = value };
    }

    public static void IsNull<T>(this FluentAssertionContext<T?> context)
        where T : class
    {
        Assert.That(context.Context, Is.Null);
    }
}