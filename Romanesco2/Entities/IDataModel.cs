using Reactive.Bindings;

namespace Romanesco2.DataModel.Entities;

internal interface IDataModel
{
    public string Title { get; }
    public IReadOnlyReactiveProperty<string> TextOfValue { get; }
}