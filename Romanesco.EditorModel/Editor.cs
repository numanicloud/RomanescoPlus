using System.Text.Json;
using System.Text.Json.Serialization;
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

    public async Task SaveAsAsync()
    {
        if (CurrentProject.Value is not { } project)
        {
            return;
        }
        
        var path = await View.PickSavePathAsync(project.DefaultSavePath.Value);
        if (path is null)
        {
            return;
        }

        var serializable = new SerializedProject()
        {
            Data = _modelFactory.MakeData(project.DataModel),
            DllPath = project.DllPath.Value.PathString
        };
        
        await using var file = path.Create();
        await JsonSerializer.SerializeAsync(file, serializable);
    }
}