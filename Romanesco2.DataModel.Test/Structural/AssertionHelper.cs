
namespace Romanesco2.DataModel.Test.Structural;

internal static class AssertionHelper
{
    public static AssertionContext<TActual?> BeginAssertion<TActual>(this TActual? obj)
    {
        return new AssertionContext<TActual?>(obj);
    }

    public static AssertionContext<TActual> NotNull<TActual>(this AssertionContext<TActual?> context)
        where TActual : class
    {
        Assert.That(context.Actual, Is.Not.Null);
        if (context.Actual is null) throw new Exception();
        return new AssertionContext<TActual>(context.Actual);
    }
}