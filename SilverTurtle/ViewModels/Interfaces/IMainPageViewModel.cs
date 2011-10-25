using System.Collections.ObjectModel;
using System.Windows.Input;
using SilverTurtle.Models;
using TurtleGraphics.EventArguments;

namespace SilverTurtle.ViewModels.Interfaces
{
    public interface IMainPageViewModel
    {
        ICommand AddSnippitCommand { get; }
        ICommand DisplayFunctionTextCommand { get; }
        ICommand AddMethodCommand { get; }
        ICommand AddColorCommand { get; }
        ICommand ShowTurtleCommand { get; }
        ICommand SelectTurtleCommand { get; }
        ICommand PrintCommand { get; }
        bool WaitForResult { get; set; }

        ObservableCollection<FunctionSourceCode> FunctionSourceCodeItems { get; }
        void SetCancel();
        void CheckProgram();
        void RunProgram();
        void StartNewProgram();
        void ClearDebugWindow();
        void SaveProgram();
        void EditProgram();
        void LoadProgram();
        void InitialiseTurtleGraphicsSystem();
        void TurtleGraphicsExecutionEngineException(object sender, ExecutionEngineErrorEventArgs e);
        void StartTurtleGraphicsSystem();
        void CentreDefaultTurtle();
    }
}