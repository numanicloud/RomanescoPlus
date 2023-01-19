using System.Windows.Markup;

namespace Romanesco.Host2.ViewModels;

[ContentProperty(nameof(Data))]
public class PropertyViewModel
{
    public required IDataViewModel Data { get; init; }
}