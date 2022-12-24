using Reactive.Bindings;

namespace Romanesco2.DataModel.Entities;

internal interface IDataModel
{
    string Title { get; }
    IReadOnlyReactiveProperty<string> TextOfValue { get; }
    IDataModel Clone();
}