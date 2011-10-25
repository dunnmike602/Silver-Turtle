using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverTurtle;
using SilverTurtle.Helpers;
using SilverTurtle.Io;
using SilverTurtle.Io.Interfaces;
using SilverTurtle.Models;
using SilverTurtle.ViewModels;
using SilverTurtle.ViewModels.Interfaces;
using TurtleGraphics;
using TurtleGraphics.Enums;
using TurtleGraphics.Interfaces;
using TurtleGraphics.VirtualMachine;

namespace SilverTurtleTest.IntegrationTests
{
    [TestClass]
    public class WhenRunningMainPageViewModel
    {
        private class FileOperationsStub : IFileOperations
        {
            public bool FileSaved { get; set; }
           
            public void SaveTextFile(Stream file, string textToSave)
            {
                FileSaved = true;
            }
        }

        private class ModalDialogStub : IModalSaveDialog
        {
            public bool? DialogResult { get; set; }

            public void Show()
            {
                
            }

            public Stream File()
            {
                return null;
            }
        }

        private const string AllCommandsText =
            @"CLEARSCREEN
                TURTLE ""A"" 1 1 1
                AT ""A""
                PENCOL 1
                HIDEALLTURTLES
                SHOWALLTURTLES
                DIAGNOSE ""TID""
                CHANGETURTLEIMAGE ""A"" 1
                HIDETURTLE ""A""
                SHOWTURTLE ""A""
                BACKCOL RED
                PENDOWN
                PENUP
                SQUARE 100
                CIRCLE 100
                TRIANGLE 100
                SETHEADING 90
                LEFT 1
                RIGHT 1
                FD 1
                MOVE 1
                BK 1
                PENW 1
                HOME
                LABEL ""TEST"" 1
                PRINT ""TEST""
                PRINTLN ""TEST""
                SX 1
                SY 1
                SETPOS 100 100
                WAIT 1
                REMARK ""THIS IS A TEST""
                ADD X 1
                SUBTRACT X 2
                RANDOM Y 2
                REPEAT 10 [FD 100]
                WHILE 2 < 1 [FD 100]
                IF 1 < 2 [FD 100]
                DIVIDE X 1
                MULTIPLY X 1
                SUBTRACT X 1
                SQUAREROOT X
                SIN X
                COS X
                TAN X
                POWER X 1
                CLEARFUNC
                STOP";

        private const string SimpleProgramText = "PENWIDTH 1 FORWARD 100";

        private const string ComplexProgramText =
            @"TO SYCAMORE2 :X
                    [ 
                     IF :X < 12 [STOP]
                     FD X
                     SET TEMP X
                    MULTIPLY TEMP 0.7
                     RT 25 
                    DO SYCAMORE2 TEMP
                    SET TEMP X
                    MULTIPLY TEMP 0.6
                     LT 25 
                    DO SYCAMORE2 TEMP
                    SET TEMP X
                    MULTIPLY TEMP 0.7
                     LT 28 
                    DO SYCAMORE2 TEMP
                     RT 28 BK X
                    ]

                    TO SYCAMORE1 X
                    [ 
                    pu home pd
                    BACKCOL LIGHTSKYBLUE
                    PENCOL DARKGREEN 
                    PU BK 300 PD
                    DO SYCAMORE2 X
                     ]

                    TO FOREST DUMMY
                     [
                    PU HOME FD 500 PD
                     DO SYCAMORE1 150
                    ]
                    DO FOREST DUMMY WAIT 5";

                            private static readonly IUnityContainer UnityContainer = new UnityContainer();

        private static ModalDialogStub _dialog;
        private static Image _turtlePointer;
        private static TextBox _debugWindow;
        private static TabItem _tiErrorTab;
        private static Color[] _colors;
        private static Canvas _drawingSurface;
        private static ScrollViewer _scrollViewer;
        private static FileOperationsStub _fileOperations;

        private static void SetupColors()
        {
            const WellKnownColors wellKnownColors = WellKnownColors.Turquoise;
            _colors = ExtensionMethods.ConvertAll((uint[])wellKnownColors.GetValues(),f => f.ToColor());
        }

        private static MainPageViewModel GetViewModelInstance()
        {
            SetupColors();

            var turtleViewModel = new TurtleViewModel();
            _dialog = new ModalDialogStub();
            _fileOperations = new FileOperationsStub();

            UnityContainer.
                RegisterInstance<IVariableHandler<float>>(FloatVariableHandler.Instance).
                RegisterInstance<ITurtleViewModel>(turtleViewModel).
                RegisterInstance<IFileOperations>(_fileOperations).
                RegisterInstance<IModalSaveDialog>(_dialog).
                RegisterInstance<ITurtleGraphicsFunctionHandler>(TurtleGraphicsFunctionHandler.Instance).
                RegisterType<IModalLoadDialog, SilverlightLoadModelDialog>().
                 RegisterType<IModalPrintDialog, SilverlightModalPrintDialog>().
                RegisterType<ICanceller, Canceller>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsExecutionEngine, TurtleGraphicsExecutionEngine>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsSyntaxAnalyser, TurtleGraphicsSyntaxAnalyser>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsLexicalAnalyser, TurtleGraphicsLexicalAnalyser>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsControlStructures, TurtleGraphicsControlStructures>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsRuntime, SilverlightTurtleGraphicsRuntime>(new ContainerControlledLifetimeManager()).
                RegisterType<ITurtleGraphicsCommandMatcher, TurtleGraphicsReflectionMatcher>(new ContainerControlledLifetimeManager()).
                RegisterType<MainPageViewModel>("MAINPAGEVIEWMODEL", new ContainerControlledLifetimeManager());

            _turtlePointer = new Image { Width = 16, Height = 16 };
            _debugWindow = new TextBox();
            _tiErrorTab = new TabItem();
            _drawingSurface = new Canvas { Width = 1000, Height = 1000 };
            _scrollViewer = new ScrollViewer();

            var parameterOverrides = new ParameterOverrides
                                         {
                                             {"drawingSurface", _drawingSurface},
                                             {"turtlePointer", _turtlePointer},
                                             {"colors", _colors},
                                             {"startForeColorIdx", 1},
                                             {"startBackColorIdx", 2},
                                             {"debugWindow", _debugWindow},
                                             {"initialPenWidth", 1},
                                             {"screenScroller", _scrollViewer},
                                         };

            var wpfTurtleGraphics = UnityContainer.Resolve<SilverlightTurtleGraphicsRuntime>(parameterOverrides);

            parameterOverrides = new ParameterOverrides
                                         {
                                             {"turtleGraphicsSystem", wpfTurtleGraphics},
                                         };

            UnityContainer.Resolve<ITurtleGraphicsCommandMatcher>(parameterOverrides);

            parameterOverrides = new ParameterOverrides
                                         {
                                             {"turtleProgramStorage", new TurtleProgramStorage()},
                                             {"turtlePointer", _turtlePointer},
                                             {"debugWindow", _debugWindow},
                                             {"tiErrorTab", _tiErrorTab},
                                             {"turtleGraphicsRuntime", wpfTurtleGraphics},
                                             {"startForeColorIndex", 1},
                                             {"initialPenWidth", 1},
                                             {"colors", _colors},
                                         };

            return UnityContainer.Resolve<MainPageViewModel>("MAINPAGEVIEWMODEL", parameterOverrides);
       }

        
        [TestMethod]
        public void CanCreateNewViewModel()
        {
            // Arrange and Act
            var viewModel = GetViewModelInstance();

            // Assert
            Assert.IsNotNull(viewModel);
        }

        [TestMethod]
        public void CanRunASimpleLogoProgram()
        {
            // Arrange
            var viewModel = GetViewModelInstance();
            viewModel.InitialiseTurtleGraphicsSystem();
            viewModel.StartTurtleGraphicsSystem();
            viewModel.StartNewProgram();
            viewModel.Model.ImmediateText = SimpleProgramText;
            viewModel.WaitForResult = true;
 
            // Act
            viewModel.RunProgram();
            
            // Assert
            Assert.AreEqual(string.Empty, viewModel.Model.ImmediateText);
            Assert.AreEqual(string.Empty, viewModel.Model.ProgramErrors);
            Assert.AreEqual(SimpleProgramText, viewModel.Model.ProcessedCommands);
            Assert.AreEqual(0, viewModel.Model.ExecutingProgramCount);
            Assert.AreEqual("AntiqueWhite", viewModel.Model.CurrentForegroundColorName);
            Assert.AreEqual("Pen Down", viewModel.Model.PenStatus);
            Assert.AreEqual("1", viewModel.Model.PenWidth);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Model.CurrentFunctionProgramText));
        }

        [TestMethod]
        public void CanCreateRunAndSaveAProgram()
        {
            // Arrange
            var viewModel = GetViewModelInstance();
            viewModel.InitialiseTurtleGraphicsSystem();
            viewModel.StartTurtleGraphicsSystem();
            viewModel.StartNewProgram();
            viewModel.Model.ImmediateText = SimpleProgramText;
            viewModel.WaitForResult = true;
            _dialog.DialogResult = true;
            _fileOperations.FileSaved = false;
 
            // Act
            viewModel.RunProgram();
            viewModel.SaveProgram();

            // Assert
            Assert.AreEqual(string.Empty, viewModel.Model.ImmediateText);
            Assert.AreEqual(true, _fileOperations.FileSaved);
            Assert.AreEqual(string.Empty, viewModel.Model.ProgramErrors);
            Assert.AreEqual(SimpleProgramText, viewModel.Model.ProcessedCommands);
            Assert.AreEqual(0, viewModel.Model.ExecutingProgramCount);
            Assert.AreEqual("AntiqueWhite", viewModel.Model.CurrentForegroundColorName);
            Assert.AreEqual("Pen Down", viewModel.Model.PenStatus);
            Assert.AreEqual("1", viewModel.Model.PenWidth);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Model.CurrentFunctionProgramText));
        }

        [TestMethod]
        public void CanRunAComplexLogoProgram()
        {
            // Arrange
            var viewModel = GetViewModelInstance();
            viewModel.InitialiseTurtleGraphicsSystem();
            viewModel.StartTurtleGraphicsSystem();
            viewModel.StartNewProgram();
            viewModel.Model.ImmediateText = ComplexProgramText;
            viewModel.WaitForResult = true;

            // Act
            viewModel.RunProgram();

            // Assert
            Assert.AreEqual(string.Empty, viewModel.Model.ImmediateText);
            Assert.AreEqual(string.Empty, viewModel.Model.ProgramErrors);
            Assert.AreEqual(ComplexProgramText, viewModel.Model.ProcessedCommands);
            Assert.AreEqual(0, viewModel.Model.ExecutingProgramCount);
            Assert.AreEqual("AntiqueWhite", viewModel.Model.CurrentForegroundColorName);
            Assert.AreEqual("Pen Down", viewModel.Model.PenStatus);
            Assert.AreEqual("1", viewModel.Model.PenWidth);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Model.CurrentFunctionProgramText));
        }

        [TestMethod]
        public void CanCheckAComplexLogoProgram()
        {
            // Arrange
            var viewModel = GetViewModelInstance();
            viewModel.InitialiseTurtleGraphicsSystem();
            viewModel.StartTurtleGraphicsSystem();
            viewModel.StartNewProgram();
            viewModel.Model.ImmediateText = ComplexProgramText;
     
            // Act
            viewModel.CheckProgram();
  
            // Assert
            Assert.AreEqual(ComplexProgramText, viewModel.Model.ImmediateText);
            Assert.AreEqual(string.Empty, viewModel.Model.ProgramErrors);
            Assert.AreEqual(ComplexProgramText, viewModel.Model.ProcessedCommands);
            Assert.AreEqual(0, viewModel.Model.ExecutingProgramCount);
            Assert.AreEqual("AntiqueWhite", viewModel.Model.CurrentForegroundColorName);
            Assert.AreEqual("Pen Down", viewModel.Model.PenStatus);
            Assert.AreEqual("1", viewModel.Model.PenWidth);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Model.CurrentFunctionProgramText));
        }

        [TestMethod]
        public void CanRunProgramWithAllCommands()
        {
            // Arrange
            var viewModel = GetViewModelInstance();
            viewModel.InitialiseTurtleGraphicsSystem();
            viewModel.StartTurtleGraphicsSystem();
            viewModel.StartNewProgram();
            viewModel.Model.ImmediateText = AllCommandsText;
            viewModel.WaitForResult = true;

            // Act
            viewModel.RunProgram();

            // Assert
            Assert.AreEqual(string.Empty, viewModel.Model.ImmediateText);
            Assert.AreEqual(string.Empty, viewModel.Model.ProgramErrors);
            Assert.AreEqual(AllCommandsText, viewModel.Model.ProcessedCommands);
            Assert.AreEqual(0, viewModel.Model.ExecutingProgramCount);
            Assert.AreEqual("AntiqueWhite", viewModel.Model.CurrentForegroundColorName);
            Assert.AreEqual("Pen Down", viewModel.Model.PenStatus);
            Assert.AreEqual("1", viewModel.Model.PenWidth);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Model.CurrentFunctionProgramText));
        }
    }
}