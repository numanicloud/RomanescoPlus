using System.Windows.Markup;
using Romanesco.DataModel.Entities.Component;

namespace Romanesco.Host2.ViewModels;

[ContentProperty(nameof(Data))]
public class PropertyViewModel
{
    public required PropertyModel Model { get; init; }
    public required IDataViewModel Data { get; init; }
}