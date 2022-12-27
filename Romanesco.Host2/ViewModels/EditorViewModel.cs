using System.Reactive.Linq;
using Reactive.Bindings;

namespace Romanesco.Host2.ViewModels;

public class EditorViewModel
{
    public IReadOnlyReactiveProperty<ProjectViewModel> Project { get; }

    public EditorViewModel()
    {
        var model = new EditorModel.Editor();

        Project = model.CurrentProject
            .Select(ToViewModel)
            .ToReadOnlyReactiveProperty(new NullProjectViewModel());

        ProjectViewModel ToViewModel(EditorModel.Project? project)
        {
            return project is not null
                ? new LoadedProjectViewModel()
                : new NullProjectViewModel();
        }
    }
}

public class ProjectViewModel
{
}

public class LoadedProjectViewModel : ProjectViewModel
{
}

public class NullProjectViewModel : ProjectViewModel
{
}