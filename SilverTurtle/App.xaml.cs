

using System;
using System.Windows;
using Microsoft.Practices.Unity;
using SilverTurtle.Io;
using SilverTurtle.Io.Interfaces;
using SilverTurtle.Locators;
using SilverTurtle.ViewModels;
using TurtleGraphics;
using TurtleGraphics.EventArguments;
using TurtleGraphics.Interfaces;
using TurtleGraphics.VirtualMachine;

namespace SilverTurtle
{
    public partial class App
    {
        private readonly IUnityContainer _unityContainer = new UnityContainer();

        public App()
        {
            Startup += ApplicationStartup;
            Exit += ApplicationExit;
            UnhandledException += ApplicationUnhandledException;

            InitializeComponent();
        }

        private void Init()
        {
            var turtleViewModel = ((ViewModelLocator)Current.Resources["Locator"]).TurtleViewModel;

            _unityContainer.
                RegisterInstance<IVariableHandler<float>>(FloatVariableHandler.Instance).
                RegisterInstance(turtleViewModel).
                RegisterInstance<ITurtleGraphicsFunctionHandler>(TurtleGraphicsFunctionHandler.Instance).
                RegisterType<ICanceller, Canceller>(new ContainerControlledLifetimeManager()).
                RegisterType<IModalSaveDialog, SilverlightSaveModelDialog>().
                RegisterType<IModalLoadDialog, SilverlightLoadModelDialog>().
                RegisterType<IFileOperations, FileOperations>().
                RegisterType<IModalPrintDialog, SilverlightModalPrintDialog>().
                RegisterType<ITurtleGraphicsExecutionEngine, TurtleGraphicsExecutionEngine>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsSyntaxAnalyser, TurtleGraphicsSyntaxAnalyser>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsLexicalAnalyser, TurtleGraphicsLexicalAnalyser>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsControlStructures, TurtleGraphicsControlStructures>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsRuntime, SilverlightTurtleGraphicsRuntime>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsCommandMatcher, TurtleGraphicsReflectionMatcher>(new ContainerControlledLifetimeManager()).
                RegisterType<MainWindow>("MAINWINDOW", new ContainerControlledLifetimeManager()).
                RegisterType<MainPageViewModel>("MAINPAGEVIEWMODEL", new ContainerControlledLifetimeManager());

            var mainWindow = _unityContainer.Resolve<MainWindow>("MAINWINDOW");

            RootVisual = mainWindow;
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            Init();
        }

        private void ApplicationExit(object sender, EventArgs e)
        {
            _unityContainer.Dispose();
        }
       
        private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
           // Attempt to report unhandled errors in the application window
            try
            {
                var mainPageViewModel = _unityContainer.Resolve<MainPageViewModel>("MAINPAGEVIEWMODEL");
                mainPageViewModel.TurtleGraphicsExecutionEngineException(null,
                                                                         new ExecutionEngineErrorEventArgs
                                                                             {Error = e.ExceptionObject});
                e.Handled = true;
            }
            catch
            {
                // Fallback to reporting the original error in the DOM
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDom(e));
            }
        }

        private static void ReportErrorToDom(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }
    }
}
