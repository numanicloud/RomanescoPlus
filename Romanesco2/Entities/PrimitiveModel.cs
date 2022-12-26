using System.Reactive.Linq;
using Reactive.Bindings;

namespace Romanesco2.DataModel.Entities;

internal abstract class PrimitiveModel<T> : IDataModel
    where T : notnull
{
    public required string Title { get; init; }
    public IReadOnlyReactiveProperty<string> TextOfValue { get; }
    public ReactiveProperty<T> Data { get; } = new();

    protected PrimitiveModel(string initialText)
    {
        if (Data is ReactiveProperty<string> str)
        {
            str.Value = initialText;
        }

        TextOfValue = Data
            .Select(x => x.ToString())
            .FilterNull()
            .ToReadOnlyReactiveProperty(initialText);
    }

    public abstract IDataModel Clone(string? title);
}