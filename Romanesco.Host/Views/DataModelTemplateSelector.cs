namespace Romanesco.Host.Views;

internal class DataModelTemplateSelector : DataTemplateSelector
{
    private readonly ResourceDictionary _inlineTemplates = new ResourceDictionary();

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return _inlineTemplates.Values
            .OfType<DataTemplate>()
            .First(x => x.Values.Any(y => y.Key.PropertyName == "Type" && (string)y.Value == item.GetType().Name));
    }

    public void AddInlineTemplate(ResourceDictionary inlineTemplates)
    {
        _inlineTemplates.Add(inlineTemplates);
    }
}