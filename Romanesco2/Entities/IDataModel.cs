using Reactive.Bindings;

namespace Romanesco.DataModel.Entities;

internal interface IDataModel
{
    string Title { get; }
    IReadOnlyReactiveProperty<string> TextOfValue { get; }
    IDataModel Clone(string? title = null);
}