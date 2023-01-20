﻿using System.Reflection;
using System.Text.Json;
using Numani.TypedFilePath;
using Reactive.Bindings;
using Numani.TypedFilePath.Infrastructure;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Factories;
using Romanesco.EditorModel.Projects;

namespace Romanesco.EditorModel;

public class Editor : IEditorCommandObserver
{
    private readonly AggregatedFactory _modelFactory;

    public required IEditorView View { private get; init; }
    public ReactiveProperty<Project?> CurrentProject { get; set; } = new();

    public Editor()
    {
        _modelFactory = new AggregatedFactory
        {
            ClassFactory = new ClassFactory
            {
                CommandObserver = this
            },
            Factories = new IModelFactory[]
            {
                new PrimitiveFactory(),
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
            DllPath = project.DllPath.Value.PathString
        };
        
        await using var file = path.Create();
        await JsonSerializer.SerializeAsync(file, serializable);
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