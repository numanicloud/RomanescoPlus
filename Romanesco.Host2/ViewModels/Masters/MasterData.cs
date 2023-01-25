using Reactive.Bindings;

namespace Romanesco.Host2.ViewModels;

internal abstract class MasterData
{
}

internal class NullMasterData : MasterData
{
}

internal class InitializedMasterData : MasterData
{
    public required ReadOnlyReactiveCollection<NamedArrayItemViewModel> Choices { get; init; }
}