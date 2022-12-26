using Reactive.Bindings;

namespace Romanesco.DataModel.Entities;

public interface IDataModel
{
    string Title { get; }
    IReadOnlyReactiveProperty<string> TextOfValue { get; }
    IDataModel Clone(string? title = null);
}