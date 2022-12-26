using Numani.TypedFilePath.Interfaces;
using Romanesco.DataModel.Entities;

namespace Romanesco.EditorModel;

internal class Project
{
    public required IAbsoluteFilePath PathLoadFrom { get; init; }
    public required IAbsoluteFilePath DllPath { get; init; }
    public required IDataModel DataModel { get; init; }
}