using Livet;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Romanesco.EditorModel;
using Romanesco.EditorModel.Projects;

namespace Romanesco.Host2.ViewModels;

public class ProjectCreationWizardViewModel : ViewModel
{
    private readonly ProjectCreationWizard _model = new ();
    private Result _result = Result.Cancelled;

    public ReactiveProperty<string> DllPath => _model.DllPath;

    public IReadOnlyReactiveProperty<string[]> TypeOptions => _model.TypeOptions;

    public ReactiveProperty<int> SelectedIndex => _model.SelectedIndex;

    public IReadOnlyReactiveProperty<bool> IsValid => _model.IsValid;

    public void Confirm()
    {
        _result = Result.Confirmed;
        Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
    }

    public void OpenDll()
    {
        Messenger.Raise(new OpeningFileSelectionMessage("OpenDll")
        {
            Filter = ".NET assembly|*.dll"
        });
    }

    public void DllSelected(OpeningFileSelectionMessage message)
    {
        if (message.Response == null || message.Response.Length < 1) return;

        DllPath.Value = message.Response[0];
    }

    public ProjectCreationResult ToResult() => _result switch
    {
        Result.Confirmed => _model.ToResult(),
        _ => new ProjectCreationCancelled()
    };

    private enum Result
    {
        Confirmed, Cancelled
    }
}