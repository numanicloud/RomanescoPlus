using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Romanesco.Host2.Views;

internal class DataModelTemplateSelector : DataTemplateSelector
{
    private readonly List<DataTemplate> _inlineTemplates2 = new List<DataTemplate>();

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return _inlineTemplates2
            .First(x => x.DataType as Type == item.GetType());
    }

    public void AddInlineTemplate(ResourceDictionary inlineTemplates)
    {
        inlineTemplates.Values
            .OfType<DataTemplate>()
            .ToList()
            .ForEach(x => _inlineTemplates2.Add(x));
    }
}