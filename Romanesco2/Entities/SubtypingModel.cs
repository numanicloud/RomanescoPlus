using System.Reactive.Linq;
using Reactive.Bindings;
using Romanesco.DataModel.Entities.Component;

namespace Romanesco.DataModel.Entities;

public sealed class SubtypingModel : IDataModel<SubtypingModel>
{
    public required string Title { get; init; }
    public required SubtypingContext Context { get; init; }
    public required TypeId[] Choices { get; init; }
    public ReactiveProperty<TypeId?> SelectedId { get; } = new((TypeId?)null);
    public ReadOnlyReactiveProperty<ClassModel?> Selected { get; }

    public SubtypingModel()
    {
        Selected = SelectedId.FilterNull()
            .Select(x => Context?.Instantiate(x, Title))
            .ToReadOnlyReactiveProperty();
    }

    public SubtypingModel CloneStrict(string? title = null)
    {
        return new SubtypingModel()
        {
            Title = title ?? Title,
            Context = Context,
            Choices = Choices
        };
    }
}