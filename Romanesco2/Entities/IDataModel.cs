namespace Romanesco.DataModel.Entities;

public interface IDataModel
{
    string Title { get; }
    IDataModel Clone(string? title = null);
}