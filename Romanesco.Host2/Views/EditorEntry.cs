using System.Windows;

namespace Romanesco.Host2.Views;

public static class EditorEntry
{
    public enum Layout
    {
        Inline, Block
    }

    public static readonly DependencyProperty LayoutProperty =
        DependencyProperty.RegisterAttached(
            "Layout",
            typeof(Layout),
            typeof(EditorEntry),
            new FrameworkPropertyMetadata(Layout.Block,
                FrameworkPropertyMetadataOptions.Inherits));

    public static Layout GetLayout(DependencyObject obj)
    {
        return (Layout)obj.GetValue(LayoutProperty);
    }

    public static void SetLayout(DependencyObject obj, Layout value)
    {
        obj.SetValue(LayoutProperty, value);
    }

    public static readonly DependencyProperty TraceProperty =
        DependencyProperty.RegisterAttached(
            "Trace",
            typeof(string),
            typeof(EditorEntry),
            new PropertyMetadata(""));

    public static string GetTrace(DependencyObject obj)
    {
        return (string)obj.GetValue(TraceProperty);
    }

    public static void SetTrace(DependencyObject obj, string value)
    {
        obj.SetValue(TraceProperty, value);
    }
}