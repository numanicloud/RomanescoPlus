using System.Reflection;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Serialization;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Factories;

public class ClassFactory : IModelFactory
{
    public required IEditorCommandObserver CommandObserver { private get; init; }

    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (!type.IsClass) return null;

        var props = from p in type.GetProperties()
            where p.GetIndexParameters().Length == 0
            where p.CanRead && p.CanWrite
            let order = p.GetCustomAttribute<EditorOrder>()?.Value ?? 0
            let model = loader.LoadType(p.Name, p.PropertyType, loader)
            where model != null
            orderby order
            select new PropertyModel()
            {
                Model = model,
                Attributes = p.GetCustomAttributes()
                    .Select(attr => new ModelAttributeData()
                    {
                        Data = attr
                    }),
                Commands = EditorCommand.ExtractCommands(type, p, model, CommandObserver)
            };

        var array = props.ToArray();

        array.ToList()
            .ForEach(item =>
            {
                if (item.Model is ArrayModel { Prototype: NamedClassModel prototype } masterProperty)
                {
                    var attr = item.Attributes.Select(x => x.Data)
                        .OfType<EditorMasterAttribute>()
                        .FirstOrDefault();
                    if (attr == null) return;

                    prototype.Inner.IdProvider = new ClassIdProvider()
                    {
                        Self = prototype.Inner,
                        PropertyName = attr.IdPropertyName,
                    };
                }
            });

        return new ClassModel()
        {
            TypeId = new TypeId(type),
            Title = title,
            Children = array
        };
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target is not ClassModel model
            || data is not SerializedClass serialized) return null;

        var result = new ClassModel()
        {
            Title = model.Title,
            TypeId = model.TypeId,
            Children = model.Children
                .Select(x => LoadMember(x.Model, serialized.Children, loader) is {} child
                    ? new PropertyModel()
                    {
                        Attributes = x.Attributes,
                        Model = child,
                        Commands = x.Commands.Select(cmd => cmd.With(child)).ToArray()
                    }
                    : x.Clone())
                .ToArray(),
        };
        if (model.IdProvider is not null)
        {
            result.IdProvider = new ClassIdProvider()
            {
                PropertyName = model.IdProvider.PropertyName,
                Self = result
            };
        }

        return result;

        static IDataModel? LoadMember(
            IDataModel targetMember,
            SerializedMember[] members,
            IModelFactory loader)
        {
            return members.FirstOrDefault(x => x.Label == targetMember.Title) is { } toLoad
                ? loader.LoadValue(targetMember, toLoad.Data, loader)
                : null;
        }
    }

    public SerializedData? MakeData(IDataModel model, IModelFactory factory)
    {
        if (model is not ClassModel c) return null;

        return new SerializedClass()
        {
            Children = c.Children.Select(x => new SerializedMember()
                {
                    Data = factory.MakeData(x.Model, factory)
                        ?? throw new InvalidOperationException(),
                    Label = x.Model.Title
                })
                .ToArray(),
        };
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        if (source is not ClassModel model
            || new TypeId(targetType) != model.TypeId) return null;

        var instance = Activator.CreateInstance(targetType);

        foreach (var child in model.Children)
        {
            if (targetType.GetProperty(child.Model.Title) is not { } p)
                throw new InvalidOperationException($"Property {targetType.Name}.{child.Model.Title} doesn't exist.");

            p.SetValue(instance, decoder.Decode(child.Model, p.PropertyType, decoder));
        }

        return instance;
    }

    public bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader)
    {
        if (source is not ClassModel model) return false;

        var type = rawValue.GetType();
        if (!type.IsClass || new TypeId(type) != model.TypeId) return false;

        foreach (var child in model.Children)
        {
            if (type.GetProperty(child.Model.Title) is not { } p) continue;

            var childValue = p.GetValue(rawValue);
            if (childValue is null) continue;

            loader.LoadRawValue(child.Model, childValue, loader);
        }

        return true;
    }
}