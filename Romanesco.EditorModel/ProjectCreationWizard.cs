using System.Reactive.Linq;
using System.Reflection;
using Numani.TypedFilePath;
using Numani.TypedFilePath.Infrastructure;
using Numani.TypedFilePath.Interfaces;
using Reactive.Bindings;
using RomanescoPlus.Annotations;

namespace Romanesco.EditorModel;

public class ProjectCreationWizard
{
    private readonly ReactiveProperty<Type[]> _typeOptions = new(Type.EmptyTypes);

    public ReactiveProperty<string> DllPath { get; } = new("");
    public IReadOnlyReactiveProperty<string[]> TypeOptions { get; }
    public ReactiveProperty<int> SelectedIndex { get; } = new();
    public IReadOnlyReactiveProperty<bool> IsValid { get; }

    public ProjectCreationWizard()
    {
        TypeOptions = _typeOptions
            .Select(x => x.Select(t => t.FullName).ToArray())
            .ToReadOnlyReactiveProperty()!;

        DllPath.Select(x => x.AsAnyPath())
            .OfType<IAbsoluteFilePathExt>()
            .Where(x => x.Extension.WithoutDot == "dll")
            .Subscribe(dll =>
            {
                _typeOptions.Value = Assembly.LoadFile(dll.PathString)
                    .GetTypes()
                    .Where(x => x.GetCustomAttributesData()
                        .Any(attr => attr.AttributeType == typeof(EditorRootAttribute)))
                    .ToArray();
            });

        IsValid = SelectedIndex.Merge(TypeOptions.Select(_ => SelectedIndex.Value))
            .Select(i => 0 <= i && i < TypeOptions.Value.Length)
            .ToReadOnlyReactiveProperty();
    }

    public ProjectCreationResult ToResult() => new ProjectCreationConfirmed(
        DllPath.Value.AssertAbsoluteFilePathExt(),
        _typeOptions.Value[SelectedIndex.Value]);
}