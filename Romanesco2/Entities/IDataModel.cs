namespace Romanesco.DataModel.Entities;

public interface IDataModel<out T> : IDataModel where T : IDataModel<T>
{
    T CloneStrict(string? title = null);
    IDataModel IDataModel.Clone(string? title) => CloneStrict(title);
}

public interface IDataModel
{
    string Title { get; }
    public IDataModel Clone(string? title);
}