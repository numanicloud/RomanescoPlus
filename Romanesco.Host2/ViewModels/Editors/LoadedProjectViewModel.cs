using Romanesco.DataModel.Entities;
using Romanesco.EditorModel.Projects;

namespace Romanesco.Host2.ViewModels;

public class LoadedProjectViewModel : ProjectViewModel
{
    public IDataViewModel Root { get; }

    public LoadedProjectViewModel(Project project)
    {
        var factory = new ViewModelFactory();
        Root = factory.Create(project.DataModel);
    }
}