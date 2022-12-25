namespace Romanesco2.DataModel.Test.Structural;

internal record SequenceAssertionContext<T>(T[] Context) : IDisposable
{
    private int _index = 0;

    public AssertionContext<T> Next()
    {
        Assert.That(_index, Is.LessThanOrEqualTo(Context.Length));

        var result = Context[_index];
        _index++;
        return new AssertionContext<T>(result);
    }

    public void Dispose()
    {
        Assert.That(_index, Is.EqualTo(Context.Length));
    }
}