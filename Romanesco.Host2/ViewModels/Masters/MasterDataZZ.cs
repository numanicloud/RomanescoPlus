using Reactive.Bindings;

namespace Romanesco.Host2.ViewModels;

internal abstract class MasterDataZZ
{
}

internal class NullMasterDataZZ : MasterDataZZ
{
}

internal class InitializedMasterDataZZ : MasterDataZZ
{
    public required ReadOnlyReactiveCollection<NamedArrayItemViewModel> Choices { get; init; }
}