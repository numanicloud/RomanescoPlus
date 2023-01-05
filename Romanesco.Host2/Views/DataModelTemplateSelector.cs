using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Romanesco.Host2.Views;

internal class DataModelTemplateSelector : DataTemplateSelector
{
    private readonly List<DataTemplateEntry> _inlineTemplates = new();

    public ObservableCollection<DataTemplateEntry> DefaultInlineTemplates { get; set; } = new();
    public ObservableCollection<DataTemplateEntry> DefaultBlockTemplates { get; set; } = new();

    public override DataTemplate SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null)
        {
            return base.SelectTemplate(item, container)!;
        }

        var source = EditorEntry.GetLayout(container) switch
        {
            EditorEntry.Layout.Inline => _inlineTemplates.Concat(DefaultInlineTemplates),
            EditorEntry.Layout.Block => DefaultBlockTemplates,
            _ => throw new Exception()
        };

        return source.Where(x => x.IsMatch(item.GetType()))
            .OrderByDescending(x => x.Priority)
            .Select(x => x.DataTemplate)
            .FirstOrDefault()!;
    }

    public void AddInlineTemplate(ResourceDictionary inlineTemplates, int priority)
    {
        inlineTemplates.Values
            .OfType<DataTemplate>()
            .ToList()
            .ForEach(x => _inlineTemplates.Add(new DataTemplateEntry()
            {
                DataTemplate = x,
                Priority = priority
            }));
    }
}

internal class DataTemplateEntry
{
    public required int Priority { get; init; }
    public required DataTemplate DataTemplate { get; init; }

    public bool IsMatch(Type type) => DataTemplate.DataType as Type == type;
}