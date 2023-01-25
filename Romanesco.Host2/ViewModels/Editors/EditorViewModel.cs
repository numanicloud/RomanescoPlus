using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Livet;
using Livet.Messaging;
using Livet.Messaging.IO;
using Numani.TypedFilePath.Infrastructure;
using Numani.TypedFilePath.Interfaces;
using Reactive.Bindings;
using Romanesco.EditorModel;
using Romanesco.EditorModel.Projects;
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

        ProjectViewModel ToViewModel(Project? project)
        {
            return project is not null
                ? new LoadedProjectViewModel(project)
                : new NullProjectViewModel();
        }
    }

    public void CreateProjectAsync() => _model.CreateProjectAsync().Forget();

    public void SaveNewProjectAsync() => _model.SaveAsAsync().Forget();

    public void LoadProjectAsync() => _model.LoadAsync().Forget();

    public async Task<ProjectCreationResult> SetupProjectCreationAsync()
    {
        var vm = new ProjectCreationWizardViewModel();
        await Messenger.RaiseAsync(new TransitionMessage(
            typeof(ProjectCreationWizard), vm, TransitionMode.Modal, "NewProject"));

        return vm.ToResult();
    }

    public async Task<IAbsoluteFilePathExt?> PickSavePathAsync(IAbsoluteFilePathExt defaultPath)
    {
        var message = new SavingFileSelectionMessage("SaveProject")
        {
            InitialDirectory = Directory.GetParent(defaultPath.PathString)!.FullName
        };

        await Messenger.RaiseAsync(message);

        if (message.Response is not { } r || r.Length < 1)
        {
            return null;
        }

        return r[0].AssertAbsoluteFilePathExt();
    }

    public async Task<IAbsoluteFilePathExt?> PickLoadPathAsync(IAbsoluteFilePathExt? defaultPath)
    {
        var message = new OpeningFileSelectionMessage("LoadProject")
        {
        };

        await Messenger.RaiseAsync(message);

        if (message.Response is not { } r || r.Length < 1) return null;

        return r[0].AssertAbsoluteFilePathExt();
    }
}