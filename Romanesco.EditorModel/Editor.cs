using Numani.TypedFilePath;
using Reactive.Bindings;
using Numani.TypedFilePath.Infrastructure;
using Romanesco.DataModel.Factories;
using Romanesco.EditorModel.Projects;

namespace Romanesco.EditorModel;

public class Editor
{
    private readonly AggregatedFactory _modelFactory = new()
    {
        ClassFactory = new ClassFactory(),
        Factories = new IModelFactory[]
        {
            new PrimitiveFactory(),
            new ArrayFactory()
        }
    };

    public required IEditorView View { private get; init; }
    public ReactiveProperty<Project?> CurrentProject { get; set; } = new();

    public async Task CreateProjectAsync()
    {
        var creation = await View.SetupProjectCreationAsync();
        if (creation is not ProjectCreationConfirmed confirmed) return;
        
        var savePath = Directory.GetParent(confirmed.DllPath.PathString)!
            .FullName
            .AssertAbsoluteDirectoryPath()
            .Combine("project.roma".AssertRelativeFilePathExt());

        var model = _modelFactory.LoadType(confirmed.Type);

        CurrentProject.Value = new Project(savePath, confirmed.DllPath)
        {
            DataModel = model
        };
    }
}