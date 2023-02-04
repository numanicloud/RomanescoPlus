using System.Reflection;
using System.Text.Json;
using Numani.TypedFilePath;
using Reactive.Bindings;
using Numani.TypedFilePath.Infrastructure;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Factories;
using Romanesco.EditorModel.Projects;
using Romanesco.DataModel.Entities.Component;

namespace Romanesco.EditorModel;

public class Editor : IEditorCommandObserver
{
    private readonly AggregatedFactory _modelFactory;

    public required IEditorView View { private get; init; }
    public ReactiveProperty<Project?> CurrentProject { get; set; } = new();

    public Editor()
    {
        var classFactory = new ClassFactory
        {
            CommandObserver = this
        };
        _modelFactory = new AggregatedFactory
        {
            ClassFactory = classFactory,
            Factories = new IModelFactory[]
            {
                new PrimitiveFactory(),
                new NamedArrayFactory(),
                new NamedClassFactory(classFactory),
                new ArrayFactory()
            }
        };
    }

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
            DllPath = project.DllPath.Value.PathString,
            RootTypeFullName = project.DataModel is ClassModel cm ? cm.TypeId.FullName : throw new InvalidOperationException()
        };

        await using var file = path.Create();
        await JsonSerializer.SerializeAsync(file, serializable);
    }

    public async Task LoadAsync()
    {
        var path = await View.PickLoadPathAsync(null);
        if (path is null) return;

        await using var file = path.OpenRead();
        var project = await JsonSerializer.DeserializeAsync<SerializedProject>(file);
        if (project is null) return;

        var dllPath = project.DllPath.AssertAbsoluteFilePathExt();
        if (!dllPath.Exists()) throw new InvalidOperationException("プロジェクトで使用するDLLが失われています");

        var assembly = Assembly.LoadFrom(dllPath.PathString);
        var type = assembly.GetType(project.RootTypeFullName);
        if (type is null) return;

        var model = _modelFactory.LoadType(type);
        model = _modelFactory.LoadValue(model, project.Data);

        CurrentProject.Value = new Project(path, dllPath)
        {
            DataModel = model
        };
    }

    public void RunCommand(EditorCommand command)
    {
        if (CurrentProject.Value is not { } project) return;
        if (!project.DllPath.Value.Exists()) throw new InvalidOperationException("コマンドを定義していたDLLが失われています");

        var assembly = Assembly.LoadFrom(project.DllPath.Value.PathString);
        var hostType = assembly.GetType(command.HostType.FullName);
        if (hostType is null) return;

        var propType = hostType.GetProperty(command.Data.Title)?.PropertyType;
        if (propType is null) return;

        var method = hostType.GetMethod(command.MethodName);
        if (method is null) return;

        var argument = _modelFactory.Decode(command.Data, propType, _modelFactory);
        if (argument is null) return;

        var result = method.Invoke(hostType, new[] { argument });
        if (result is null) return;

        _modelFactory.LoadRawValue(command.Data, result);
    }
}