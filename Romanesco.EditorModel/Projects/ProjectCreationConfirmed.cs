using Numani.TypedFilePath.Interfaces;

namespace Romanesco.EditorModel.Projects;

public record ProjectCreationConfirmed(IAbsoluteFilePathExt DllPath, Type Type) : ProjectCreationResult;