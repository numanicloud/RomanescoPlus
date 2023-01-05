using Numani.TypedFilePath.Interfaces;

namespace Romanesco.EditorModel;

public record ProjectCreationConfirmed(IAbsoluteFilePathExt DllPath, Type Type) : ProjectCreationResult;