using Reactive.Bindings;

namespace Romanesco.EditorModel;

public class Editor
{
    public ReactiveProperty<Project?> CurrentProject { get; set; } = new();
}