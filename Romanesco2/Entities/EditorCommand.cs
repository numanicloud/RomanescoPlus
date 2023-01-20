using System.Reflection;
using Reactive.Bindings;
using Romanesco.DataModel.Factories;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Entities;

public interface IEditorCommandObserver
{
    void RunCommand(EditorCommand command);
}

public class EditorCommand
{
    public required TypeId HostType { get; init; }
    public required string MethodName { get; init; }
    public required IDataModel Data { get; init; }
    public required IEditorCommandObserver Observer { get; init; }
    public ReactiveCommand RunCommand { get; }

    public EditorCommand()
    {
        RunCommand = new ReactiveCommand();
        RunCommand.Subscribe(_ => Run());
    }

    public void Run()
    {
        Observer.RunCommand(this);
    }

    public void Run(Assembly assembly, IModelFactory factory)
    {
        var type = assembly.GetType(HostType.Name);
        if (type is null) return;

        var propType = type.GetProperty(Data.Title)?.PropertyType;
        if (propType is null) return;

        var method = type.GetMethod(MethodName);
        if (method is null) return;

        var argument = factory.Decode(Data, propType, factory);
        if (argument is null) return;

        var result = method.Invoke(type, new[] { argument });
        if (result is null) return;

        factory.LoadRawValue(Data, result, factory);
    }

    public static EditorCommand[] ExtractCommands(
        Type hostType,
        PropertyInfo propertyInfo,
        IDataModel propertyModel,
        IEditorCommandObserver observer)
    {
        return propertyInfo.GetCustomAttributes()
            .OfType<EditorCommandTargetAttribute>()
            .Where(x =>
            {
                var method = hostType.GetMethod(x.CommandName);
                if (method is null || !method.IsStatic || !method.IsPublic) return false;

                var returnType = method.ReturnType;
                var parameters = method.GetParameters();
                var propertyType = propertyInfo.PropertyType;
                if (parameters is not [var input]) return false;

                return returnType == input.ParameterType
                    && input.ParameterType == propertyType;
            })
            .Select(x => new EditorCommand()
            {
                MethodName = x.CommandName,
                HostType = new TypeId(hostType),
                Data = propertyModel,
                Observer = observer
            }).ToArray();
    }
}
