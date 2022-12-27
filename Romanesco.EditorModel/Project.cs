using Numani.TypedFilePath.Interfaces;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.EditorModel;

public class Project
{
    public ReactiveProperty<IAbsoluteFilePathExt> DefaultSavePath { get; }
    public ReactiveProperty<IAbsoluteFilePathExt> DllPath { get; }
    public required IDataModel DataModel { get; init; }

    public Project(IAbsoluteFilePathExt defaultSavePath, IAbsoluteFilePathExt dllPath)
    {
        DefaultSavePath = new ReactiveProperty<IAbsoluteFilePathExt>(defaultSavePath);
        DllPath = new ReactiveProperty<IAbsoluteFilePathExt>(dllPath);
    }
}