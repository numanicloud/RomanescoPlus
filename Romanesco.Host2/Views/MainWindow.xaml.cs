using System;
using System.Windows;

namespace Romanesco.Host2.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var resources = new ResourceDictionary()
        {
            Source = new Uri("pack://application:,,,/Views/EditorTemplates.xaml")
        };
        TemplateSelector.AddInlineTemplate(resources);
    }
}