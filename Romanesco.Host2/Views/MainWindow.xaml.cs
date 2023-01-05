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
        
        var selector = (DataModelTemplateSelector)Resources["TemplateSelector"];
    }
}