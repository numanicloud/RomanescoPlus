using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Livet;
using Livet.Messaging;
using Reactive.Bindings;
using Romanesco.EditorModel;
using ProjectCreationWizard = Romanesco.Host2.Views.ProjectCreationWizard;

namespace Romanesco.Host2.ViewModels;

public class EditorViewModel : ViewModel, IEditorView
{
    private readonly EditorModel.Editor _model;

    public IReadOnlyReactiveProperty<ProjectViewModel> Project { get; }

    public EditorViewModel()
    {
        _model = new EditorModel.Editor()
        {
            View = this
        };

        Project = _model.CurrentProject
            .Select(ToViewModel)
            .ToReadOnlyReactiveProperty(new NullProjectViewModel());

        ProjectViewModel ToViewModel(EditorModel.Project? project)
        {
            return project is not null
                ? new LoadedProjectViewModel()
                : new NullProjectViewModel();
        }
    }

    public void CreateProjectAsync() => _model.CreateProjectAsync().Forget();

    public async Task<ProjectCreationResult> SetupProjectCreationAsync()
    {
        var vm = new ProjectCreationWizardViewModel();
        await Messenger.RaiseAsync(new TransitionMessage(
            typeof(ProjectCreationWizard), vm, TransitionMode.Modal, "NewProject"));

        return vm.ToResult();
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