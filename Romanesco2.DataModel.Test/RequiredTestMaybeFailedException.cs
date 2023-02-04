namespace Romanesco.DataModel.Test;

internal class RequiredTestMaybeFailedException : Exception
{
    public RequiredTestMaybeFailedException()
        : base("前提となるテストが失敗している可能性があります")
    {
    }
}