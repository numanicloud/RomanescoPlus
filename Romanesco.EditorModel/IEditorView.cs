using Numani.TypedFilePath.Interfaces;
using Romanesco.EditorModel.Projects;

namespace Romanesco.EditorModel;

public interface IEditorView
{
    Task<ProjectCreationResult> SetupProjectCreationAsync();
    Task<IAbsoluteFilePathExt?> PickSavePathAsync(IAbsoluteFilePathExt defaultPath);
}