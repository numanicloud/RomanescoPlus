using System.Reflection;
using Reactive.Bindings;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Entities.Component;

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

    public EditorCommand With(IDataModel data)
    {
        return new EditorCommand()
        {
            HostType = HostType,
            MethodName = MethodName,
            Data = data,
            Observer = Observer
        };
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
