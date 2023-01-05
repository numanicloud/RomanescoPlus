using Romanesco.EditorModel.Projects;

namespace Romanesco.EditorModel;

public interface IEditorView
{
    Task<ProjectCreationResult> SetupProjectCreationAsync();
}