using Romanesco.Host.Views;

namespace Romanesco.Host;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        DataModelTemplateSelector.AddInlineTemplate(new EditorTemplates());
    }
}