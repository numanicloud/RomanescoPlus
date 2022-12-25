namespace Romanesco2.DataModel.Test.Fluent;

internal class SequenceAssertionContext<T> : IDisposable
{
    public required T[] Context { get; init; }

    private int _index = 0;

    public FluentAssertionContext<T> Next()
    {
        Assert.That(_index, Is.LessThanOrEqualTo(Context.Length));

        var result = Context[_index];
        _index++;
        return new FluentAssertionContext<T>()
        {
            Context = result
        };
    }

    public void Dispose()
    {
        Assert.That(_index, Is.EqualTo(Context.Length));
    }
}