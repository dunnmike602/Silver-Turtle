using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverTurtle;
using SilverTurtle.Helpers;
using SilverTurtle.ViewModels;
using SilverTurtle.ViewModels.Interfaces;
using TurtleGraphics;
using TurtleGraphics.Constants;
using TurtleGraphics.Enums;
using TurtleGraphics.Exceptions;

namespace SilverTurtleTest.UnitTests
{
    [TestClass]
    public class WhenRunningSilverlightRuntime
    {
        private const int StartupPrimitiveCount = 2;
        private const string SampleForeGroundColorString = "#FF7FFFD4";
        private const string InitialForeGroundColorString ="#FFF0F8FF";
        private const int SampleForeGroundColorIdx = 3;
        private const int ForwardIncrement = 100;
        private const int TurnIncrement = 90;
        private const int SamplePenWidth = 2;
        private const string SampleLabelText = "Some Text";
        private const string NewTurtleName = "new turtle";

        private static Canceller _canceller;
        private static Canvas _drawingSurface;
        private static Image _turtlePointer;
        private static Color[] _colors;
        private static TextBox _debugWindow;
        private static ScrollViewer _scrollViewer;
        private static ITurtleViewModel _turtleViewModel;

        public WhenRunningSilverlightRuntime()
        {
            _canceller = new Canceller();
            _drawingSurface = new Canvas {Width = 1000, Height = 1000};
            _turtlePointer = new Image {Width = 16, Height = 16};

            const WellKnownColors wellKnownColors = WellKnownColors.Turquoise;
            _colors = ExtensionMethods.ConvertAll((uint[])wellKnownColors.GetValues(),
                                               f => f.ToColor());
            _debugWindow = new TextBox();
            _scrollViewer = new ScrollViewer();
            _turtleViewModel = new TurtleViewModel();
        }

        private static SilverlightTurtleGraphicsRuntime GetRuntime()
        {
            return new SilverlightTurtleGraphicsRuntime(_canceller, _drawingSurface, _turtlePointer, _colors, 0,
                                                        0,
                                                        _debugWindow, 0, _turtleViewModel, _scrollViewer);
        }

        [TestMethod]
        public void CanCreateNewRuntime()
        {
            // Arrange and Act
            var runtime = GetRuntime();

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanCenterTurtle()
        {
            // Arrange
            var runtime = GetRuntime();
          
            // Act
            runtime.Forward(100);
            runtime.CenterTurtle();

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 2, runtime.PrimitiveCount);
  }

        [TestMethod]
        public void CanSetLabel()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.Label(SampleLabelText, 1);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanPrintToDebugWindows()
        {
            // Arrange
            var runtime = GetRuntime();
            _debugWindow.Text = string.Empty;

            // Act
            runtime.Print(SampleLabelText);

            // Assert
            Assert.AreEqual(_debugWindow.Text, SampleLabelText);
        }

        [TestMethod]
        public void CanPrintLineToDebugWindows()
        {
            // Arrange
            var runtime = GetRuntime();
            _debugWindow.Text = string.Empty;

            // Act_debugw

            runtime.PrintLine(SampleLabelText);

            // Assert
            Assert.AreEqual(_debugWindow.Text, SampleLabelText + Environment.NewLine);
        }

        [TestMethod]
        public void CanSelectPenUp()
        {
            // Arrange
            var runtime = GetRuntime();
            var isUp = false;
            runtime.PenStatusChanged += (o, e) => { isUp = e.IsUp; };

            // Act
            runtime.PenUp();

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(true, isUp);
        }

        [TestMethod]
        public void CanSetPenWidth()
        {
            // Arrange
            var runtime = GetRuntime();
            var penWidth = 0d;

            runtime.PenWidthChanged += (o, e) => { penWidth = e.Width; };

            // Act
            runtime.PenWidth(SamplePenWidth);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(penWidth, SamplePenWidth);
        }

        [TestMethod]
        public void CanSelectPenDown()
        {
            // Arrange
            var runtime = GetRuntime();
            var isUp = true;
            runtime.PenStatusChanged += (o, e) => { isUp = e.IsUp; };

            // Act
            runtime.PenDown();

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(false, isUp);
        }

        [TestMethod]
        public void CanDrawSquare()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.DrawSquare(1);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanClearScreen()
        {
            // Arrange
            var runtime = GetRuntime();
            var hasCleared = false;
            runtime.Cleared += (o, e) => { hasCleared = true; };

            // Act
            runtime.DrawSquare(1);
            runtime.ClearScreen();

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(true, hasCleared);
        }

        [TestMethod]
        public void CanTurnLeft()
        {
            // Arrange
            var runtime = GetRuntime();
            runtime.Rotate(0);
            var originalLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Act
            runtime.Left(TurnIncrement);
            runtime.Forward(ForwardIncrement);
            var currentLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.AreEqual(Convert.ToInt32(originalLocation.X) - ForwardIncrement, Convert.ToInt32(currentLocation.X));
            Assert.AreEqual(Convert.ToInt32(originalLocation.Y), Convert.ToInt32(currentLocation.Y));
        }

        [TestMethod]
        public void CanTurnRight()
        {
            // Arrange
            var runtime = GetRuntime();
            runtime.Rotate(0);
            var originalLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Act
            runtime.Right(TurnIncrement);
            runtime.Forward(ForwardIncrement);
            var currentLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.AreEqual(Convert.ToInt32(originalLocation.X) + ForwardIncrement, Convert.ToInt32(currentLocation.X));
            Assert.AreEqual(Convert.ToInt32(originalLocation.Y), Convert.ToInt32(currentLocation.Y));
        }

        [TestMethod]
        public void SquareNotDrawnWhenPenUp()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.PenUp();
            runtime.DrawSquare(1);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanDrawCircle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.DrawCircle(1);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CircleNotDrawnWhenPenUp()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.PenUp();
            runtime.DrawCircle(1);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanDrawTriangle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.DrawTriangle(1);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void TriangleNotDrawnWhenPenUp()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.PenUp();
            runtime.DrawTriangle(1);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanCreateTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.Turtle("NewTurtle", 0,0,0);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.AreEqual(2, runtime.TurtleCount);
        }

        [TestMethod]
        public void CanDeleteTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.Turtle("NewTurtle", 0, 0, 0);
            runtime.DeleteTurtle("NewTurtle");

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(1, runtime.TurtleCount);
        }

        [TestMethod]
        [ExpectedException(typeof(TurtleRuntimeException))]
        public void ThrowsWhenTurtleBeingDeletedDoesNotExist()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.DeleteTurtle("NewTurtle");
        }

        [TestMethod]
        [ExpectedException(typeof(TurtleRuntimeException))]
        public void ThrowsWhenDeletingDefaultTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.DeleteTurtle(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);
        }

        [TestMethod]
        [ExpectedException(typeof(TurtleRuntimeException))]
        public void ThrowsWhenCreatingDuplicateTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.Turtle(NewTurtleName, 0, 0, 0);
            runtime.Turtle(NewTurtleName, 0, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(TurtleRuntimeException))]
        public void ThrowsWhenSettingNonExistantTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.SetActiveTurtle(NewTurtleName);
        }

        [TestMethod]
        public void CanSetNewTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.ClearScreen();
            runtime.Turtle(NewTurtleName, 0, 0, 0);
            runtime.SetActiveTurtle(NewTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.AreEqual(NewTurtleName, SilverlightTurtleGraphicsRuntime.CurrentTurtleName);
        }

        [TestMethod]
        public void CanChangeTurtleImage()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.ClearScreen();
            runtime.Turtle(NewTurtleName, 0, 0, 0);
            runtime.SetActiveTurtle(NewTurtleName);
            runtime.ChangeTurtleImage(NewTurtleName, 1);
            var img = runtime.GetCurrentTurtleImage(SilverlightTurtleGraphicsRuntime.CurrentTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.IsNotNull(img.Source);
        }

        [TestMethod]
        public void CanShowTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.ClearScreen();
            runtime.Turtle(NewTurtleName, 0, 0, 0);
            runtime.SetActiveTurtle(NewTurtleName);
            runtime.ShowTurtle(NewTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanHideAllTurtles()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.ClearScreen();
            runtime.Turtle(NewTurtleName, 0, 0, 0);
            runtime.SetActiveTurtle(NewTurtleName);
            runtime.HideAllTurtles();

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }

        [TestMethod]
        public void CanShowAllTurtles()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.ClearScreen();
            runtime.Turtle(NewTurtleName, 0, 0, 0);
            runtime.SetActiveTurtle(NewTurtleName);
            runtime.ShowAllTurtles();

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }
        [TestMethod]
        public void CanHideTurtle()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.ClearScreen();
            runtime.Turtle(NewTurtleName, 0, 0, 0);
            runtime.SetActiveTurtle(NewTurtleName);
            runtime.HideTurtle(NewTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
        }
        [TestMethod]
        public void CanDiagnoseThreadId()
        {
            // Arrange
            var runtime = GetRuntime();
            _debugWindow.Text = string.Empty;

            // Act
            runtime.Diagnose(GlobalConstants.DiagnoseThreadId);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(_debugWindow.Text, "Managed Thread Id=" + Thread.CurrentThread.ManagedThreadId + Environment.NewLine);
        }

        [TestMethod]
        public void CanMoveForeward()
        {
            // Arrange
            var runtime = GetRuntime();
            var originalLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Act
            runtime.Rotate(0);
            runtime.Forward(ForwardIncrement);
            var currentLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.AreEqual(Convert.ToInt32(originalLocation.X), Convert.ToInt32(currentLocation.X));
            Assert.AreEqual(Convert.ToInt32(originalLocation.Y) - ForwardIncrement, Convert.ToInt32(currentLocation.Y));
        }

        [TestMethod]
        public void CanSetPos()
        {
            // Arrange
            var runtime = GetRuntime();
         
            // Act
            runtime.SetPos(100,100);
            var currentLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.AreEqual(100, Convert.ToInt32(currentLocation.X));
            Assert.AreEqual(100, Convert.ToInt32(currentLocation.Y));
        }

        [TestMethod]
        public void CanSetXandYPos()
        {
            // Arrange
            var runtime = GetRuntime();

            // Act
            runtime.SetX(100);
            runtime.SetY(100);
            var currentLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 2, runtime.PrimitiveCount);
            Assert.AreEqual(100, Convert.ToInt32(currentLocation.X));
            Assert.AreEqual(100, Convert.ToInt32(currentLocation.Y));
        }

        [TestMethod]
        public void CanMoveTurtle()
        {
            // Arrange
            var runtime = GetRuntime();
            var originalLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Act
            runtime.Rotate(0);
            runtime.Move(ForwardIncrement);
            var currentLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(Convert.ToInt32(originalLocation.X), Convert.ToInt32(currentLocation.X));
            Assert.AreEqual(Convert.ToInt32(originalLocation.Y) - ForwardIncrement, Convert.ToInt32(currentLocation.Y));
        }
        [TestMethod]
        public void CanMoveBackward()
        {
            // Arrange
            var runtime = GetRuntime();
            var originalLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Act
            runtime.Rotate(0);
            runtime.Backward(ForwardIncrement);
            var currentLocation = runtime.GetCurrentTurtlePos(SilverlightTurtleGraphicsRuntime.DefaultTurtleName);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount + 1, runtime.PrimitiveCount);
            Assert.AreEqual(Convert.ToInt32(originalLocation.X), Convert.ToInt32(currentLocation.X));
            Assert.AreEqual(Convert.ToInt32(originalLocation.Y) + ForwardIncrement, Convert.ToInt32(currentLocation.Y));
        }

        [TestMethod]
        public void CanSelectPen()
        {
            // Arrange
            var runtime = GetRuntime();
            var isForeGround = false;
            var newColor = Colors.Black;
            var oldColor = Colors.Black;

            runtime.ColorChanged += (o, e) =>
                                        {
                                            isForeGround = e.IsForeground;
                                            newColor = e.NewColor;
                                            oldColor = e.OldColor;
                                        };
        
            // Act
            runtime.SelectPen(SampleForeGroundColorIdx);

            // Assert
            Assert.AreEqual(StartupPrimitiveCount, runtime.PrimitiveCount);
            Assert.AreEqual(true, isForeGround);
            Assert.AreEqual(SampleForeGroundColorString, newColor.ToString());
            Assert.AreEqual(InitialForeGroundColorString, oldColor.ToString());
        }
    }
}