#region SILVERTURTLE - GPL Copyright (c) 2011 MLD Computing Limited
//
// This file is part of SILVERTURTLE.
//
// SILVERTURTLE is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// SILVERTURTLE is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SilverTurtle; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//================================================================================
#endregion
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media.Imaging;
using SilverTurtle.Helpers;
using SilverTurtle.ViewModels.Interfaces;
using TurtleGraphics.Constants;
using TurtleGraphics.Delegates;
using TurtleGraphics.Enums;
using TurtleGraphics.EuclidianShapes;
using TurtleGraphics.EventArguments;
using TurtleGraphics.Exceptions;
using TurtleGraphics.Helpers;
using TurtleGraphics.Interfaces;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using TurtleGraphics.VirtualMachine;
using TurtleWpf.Helpers;
using System.Linq;
using ResourceHelper = SilverTurtle.Helpers.ResourceHelper;

namespace SilverTurtle
{
    public class SilverlightTurtleGraphicsRuntime : ITurtleGraphicsRuntime
    {
        public event ClearedEventHandler Cleared;
        public event PenStatusEventHandler PenStatusChanged;
        public event ColorChangeEventHandler ColorChanged;
        public event PenWidthEventHandler PenWidthChanged;

        #region Turtle Specific Attributes

        public const string DefaultTurtleName = "default";

        private Dictionary<string, double> _penWidths;
        private Dictionary<string, Point> _currentLocations;
        private Dictionary<string, Image> _turtlePointers;
        private Dictionary<string, Color> _penColors;
        private Dictionary<string, bool> _penUpFlags;
        private Dictionary<string, int> _rotationAngles;

        /// <summary>
        /// This property is marked with the ThreadStaticAttribute so that each thread of execution gets its own value of
        /// it. This means that threads have affinity to turtles so that multiple programs can be run together that
        /// independantly control different turtles.
        /// </summary>
        [ThreadStaticAttribute]
        private static string _currentThreadSpecificTurtleName;

        public static string CurrentTurtleName
        {
            get { return string.IsNullOrEmpty(_currentThreadSpecificTurtleName) ? DefaultTurtleName : _currentThreadSpecificTurtleName; }
            set { _currentThreadSpecificTurtleName = value; }
        }

        #endregion

        private readonly Color[] _colors;
        private readonly int _startForeColorIdx;
        private readonly int _startBackColorIdx;
        private readonly TextBox _debugWindow;
        private readonly int _initialPenWidth;
        private readonly ITurtleViewModel _turtleViewModel;
        private readonly ScrollViewer _screenScroller;
        private readonly ICanceller _canceller;
        private readonly Panel _drawingSurface;
        private Path _gridPath;
        private readonly AutoResetEvent  _waitCanceller = new AutoResetEvent(false);
        private readonly Image _defaultTurtlePointer;
        public Color BackgroundColor { get; set; }

        private void SetupTurtleTipsHelper()
        {
            foreach (var pointer in _turtlePointers)
            {
                var tt = ToolTipService.GetToolTip(pointer.Value) as ToolTip;

                string tip = pointer.Key + ": Pen is " + ColorHelper.GetKnownColorName(_penColors[pointer.Key])
                    + ", Pen is " + (_penUpFlags[pointer.Key] ? "up" : "down") + ", (x,y) = (" +
                    Convert.ToInt32(_currentLocations[pointer.Key].X) + "," + Convert.ToInt32(_currentLocations[pointer.Key].Y) + ")";

                if (tt == null)
                {
                    ToolTipService.SetToolTip(pointer.Value, tip);
                }
                else
                {
                    tt.Content = tip;
                }
            }
        }

        private void SetupTurtleTips()
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                SetupTurtleTipsHelper();
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(SetupTurtleTipsHelper);
            }
        }

        private Image CreateImageHelper(int turtleImageIndex)
        {
            var turtle = _turtleViewModel.Turtles.Where(m => m.Id == turtleImageIndex).FirstOrDefault();

            var turtleImageName = turtle != null ? turtle.Image : _turtleViewModel.Turtles[0].Image;

            var newturtleBitmapImage = new BitmapImage();
            newturtleBitmapImage.SetSource(ResourceHelper.GetApplicationResourceStream(turtleImageName));

            return new Image
            {
                Source = newturtleBitmapImage,
                Height = 28,
                Width = 28,
                Margin = new Thickness(0, 0, 0, 0)
            };
        }

        private void CreateTurtleHelper(string turtleName, int turtleImageIndex)
        {
            turtleName = turtleName.TrimQuotes().ToLower();

            var newTurtleImage = CreateImageHelper(turtleImageIndex);

            _turtlePointers[turtleName] = newTurtleImage;

            SelectPenHelper(_startForeColorIdx, turtleName);
            _drawingSurface.Children.Add(_turtlePointers[turtleName]);

            MoveTurtle(turtleName);
            Canvas.SetZIndex(_turtlePointers[turtleName], 100);
        }

        private void ChangeTurtleImageHelper(string turtleName, int turtleImageIndex)
        {
            _turtlePointers[turtleName].Source = CreateImageHelper(turtleImageIndex).Source;
        }

        private void SetupTurtleDataStructures(string turtleName, Image turtleImage, Point startPoint,
            int initialPenWidth)
        {
            _turtlePointers.Add(turtleName, turtleImage);
            _penWidths.Add(turtleName, initialPenWidth);
            _rotationAngles.Add(turtleName, 0);
            _currentLocations.Add(turtleName, startPoint);
            _penUpFlags.Add(turtleName, false);
            _penColors.Add(turtleName, Colors.Black);
        }

        public Point GetCurrentTurtlePos(string turtleName)
        {
            return _currentLocations[turtleName];
        }

        public Image GetCurrentTurtleImage(string turtleName)
        {
            return _turtlePointers[turtleName];
        }

        public void Turtle(string turtleName, int turtleImage, int startPosX, int startPosY)
        {
            if (_turtlePointers.ContainsKey(turtleName.TrimQuotes().ToLower()))
            {
                throw new TurtleRuntimeException(string.Format(ResourceHelper.GetStaticText("DuplicateTurtle"), turtleName));
            }

            SetupTurtleDataStructures(turtleName.TrimQuotes().ToLower(), null, new Point(startPosX, startPosY), _initialPenWidth);

            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                CreateTurtleHelper(turtleName, turtleImage);
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => CreateTurtleHelper(turtleName, turtleImage));
            }
        }

        public void Diagnose(string runtimeProperty)
        {
            runtimeProperty = runtimeProperty.TrimQuotes().ToLower();

            if(runtimeProperty == GlobalConstants.DiagnoseThreadId)
            {
                PrintLine("Managed Thread Id=" + Thread.CurrentThread.ManagedThreadId);
            }
        }

        private void EnsureTurtle(string turtleName)
        {
            if (!_turtlePointers.ContainsKey(turtleName))
            {
                throw new TurtleRuntimeException(string.Format(ResourceHelper.GetStaticText("TurtleDoesNotExist"),
                                                               turtleName));
            }
        }

        public void ChangeTurtleImage(string turtleName, int turtleImageIndex)
        {
            turtleName = turtleName.TrimQuotes().ToLower();

            EnsureTurtle(turtleName);
            
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                ChangeTurtleImageHelper(turtleName, turtleImageIndex);
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => ChangeTurtleImageHelper(turtleName, turtleImageIndex));
            }
        }

        public void SetActiveTurtleHelper(string turtleName)
        {
            CurrentTurtleName = turtleName;
        }

        public void SetActiveTurtle(string turtleName)
        {
            turtleName = turtleName.TrimQuotes().ToLower();

            EnsureTurtle(turtleName);

            // This call must never be run on the UI Thread
            SetActiveTurtleHelper(turtleName);
        }

        public int TurtleCount
        {
            get { return _turtlePointers.Count; }
        }

        public int PrimitiveCount
        {
            get { return _drawingSurface.Children.Count; }
        }

        private void DrawGrid()
        {
            _gridPath = new Path
            {
                Stroke = ColorHelper.GetSolidColorBrush(Colors.LightGray),
                StrokeThickness = 1,
                Data = new GeometryGroup(),
            };
    
            _drawingSurface.Children.Add(_gridPath);

            Canvas.SetZIndex(_gridPath, 500);

            for (var x = 0; x < _drawingSurface.Width; x += 50)
            {
                var line = new LineGeometry
                {
                    StartPoint = new Point(x, 0),
                    EndPoint = new Point(x, _drawingSurface.Height)
                };

                ((GeometryGroup)_gridPath.Data).Children.Add(line);
            }

            for (var y = 0; y < _drawingSurface.Height; y += 50)
            {
                var line = new LineGeometry
                {
                    StartPoint = new Point(0, y),
                    EndPoint = new Point(_drawingSurface.Width, y)
                };

                ((GeometryGroup)_gridPath.Data).Children.Add(line);
            }
        }

        private void DrawSquareHelper(double length, string turtleName)
        {
            length = Math.Abs(length);

            var square = new Rectangle
            {
                Stroke = ColorHelper.GetSolidColorBrush(_penColors[turtleName]),
                StrokeThickness = _penWidths[turtleName],
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = length,
                Height = length,
                Margin = new Thickness(_currentLocations[turtleName].X - length / 2,
                                       _currentLocations[turtleName].Y - length / 2, 0, 0),
                RenderTransform = new RotateTransform
                {
                    Angle = _rotationAngles[turtleName],
                    CenterX = length / 2,
                    CenterY = length / 2
                },
            };

            AddToDrawingSurface(square);
        }

        private void AddToDrawingSurface(UIElement element)
        {
            _drawingSurface.Children.Add(element);
        }

        private void DrawTriangleHelper(double length, string turtleName)
        {
            length = Math.Abs(length);

            var p1 = new Point(_currentLocations[turtleName].X - length / 2, _currentLocations[turtleName].Y + length / 2);
            var p2 = new Point(_currentLocations[turtleName].X + length / 2, _currentLocations[turtleName].Y + length / 2);
            var p3 = new Point(_currentLocations[turtleName].X, _currentLocations[turtleName].Y - length / 2);

            var triangle = new Polygon
            {
                Points = new PointCollection { p1, p2, p3 },
                Stroke = ColorHelper.GetSolidColorBrush(_penColors[turtleName]),
                StrokeThickness = _penWidths[turtleName],
                RenderTransform = new RotateTransform
                {
                    Angle = _rotationAngles[turtleName],
                    CenterX = _currentLocations[turtleName].X,
                    CenterY = _currentLocations[turtleName].Y,
                },
            };

            AddToDrawingSurface(triangle);
        }

        private void DrawCircleHelper(double diameter, string turtleName)
        {
            diameter = Math.Abs(diameter);

            var circle = new Ellipse
            {
                Stroke = ColorHelper.GetSolidColorBrush(_penColors[turtleName]),
                StrokeThickness = _penWidths[turtleName],
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = diameter,
                Height = diameter,
                Margin = new Thickness(_currentLocations[turtleName].X - diameter / 2,
                                       _currentLocations[turtleName].Y - diameter / 2, 0, 0),
            };

            AddToDrawingSurface(circle);
        }

        private void DrawInDirection(int length, double rotationAngle, string turtleName)
        {
            var nextLine = new EuclidianLine
            {
                StartPoint = _currentLocations[turtleName],
                Length = length
            };

            var endPoint = nextLine.SetEndLocationFromAngle(rotationAngle);

            if (!_penUpFlags[turtleName])
            {
                DrawLine(_currentLocations[turtleName], endPoint, turtleName);
            }

            _currentLocations[turtleName] = endPoint;
        }

        private void SetLabelHelper(string text, float fontSize, string turtleName)
        {
            text = text.TrimQuotes();
         
            var textBlock = new TextBlock
            {
                Text = text,
                Foreground = ColorHelper.GetSolidColorBrush(_penColors[turtleName]),
                FontSize = fontSize,
                RenderTransform = new RotateTransform
                {
                    Angle = _rotationAngles[turtleName] - 
                    GlobalConstants.NinetyDegrees, }
            };

            Canvas.SetLeft(textBlock, _currentLocations[turtleName].X);
            Canvas.SetTop(textBlock, _currentLocations[turtleName].Y);

            AddToDrawingSurface(textBlock);
        }

        private Point GetCentrePoint()
        {
            return new Point(_screenScroller.ViewportWidth / 2, _screenScroller.ViewportHeight / 2);
        }

        private void SetToCentre(string turtleName)
        {
            _currentLocations[turtleName] = GetCentrePoint();

            MoveTurtle(turtleName);

            _screenScroller.ScrollToTop();
            _screenScroller.ScrollToLeft();
        }

        private void DrawToCenter(string turtleName)
        {
            var homePosition = GetCentrePoint();

            if (!_penUpFlags[turtleName])
            {
                DrawLine(_currentLocations[turtleName], homePosition, turtleName);
            }

            SetPosHelper((int)homePosition.X, (int)homePosition.Y, turtleName, false);
        }

        private void DrawLine(Point from, Point to, string turtleName)
        {
            var path = GetPath(turtleName);

            ((GeometryGroup)path.Data).Children.Add(new LineGeometry { StartPoint = from, EndPoint = to, });
        }

        private Path GetPath(string turtleName)
        {
            var currentBrush = ColorHelper.GetSolidColorBrush(_penColors[turtleName]);

            var path = new Path
                           {
                               Stroke = currentBrush,
                               StrokeThickness = _penWidths[turtleName],
                               Data = new GeometryGroup(),
                           };
           
            AddToDrawingSurface(path);

            return path;
        }

        private void InitialiseDictionaries()
        {
            _penWidths = new Dictionary<string, double>();
            _currentLocations = new Dictionary<string, Point>(    );
            _turtlePointers = new Dictionary<string, Image>();
            _penColors = new Dictionary<string, Color>();
            _penUpFlags = new Dictionary<string, bool>();
            _rotationAngles = new Dictionary<string, int>();
        }

        private void InitialiseDefaultTurtle()
        {
            CurrentTurtleName = DefaultTurtleName;

            SetupTurtleDataStructures(CurrentTurtleName, _defaultTurtlePointer, GetCentrePoint(), _initialPenWidth);

            SetToCentre(CurrentTurtleName);
            RotateTurtle(0, CurrentTurtleName);
            PenDown();

            SelectPen(_startForeColorIdx);

            _drawingSurface.Children.Clear();
            _drawingSurface.Children.Add(_turtlePointers[CurrentTurtleName]);
            Canvas.SetZIndex(_turtlePointers[CurrentTurtleName], 100);
        }

        private void InitialiseStructures()
        {
            CancelWait();

            _canceller.SetCancel(true);

            InitialiseDictionaries();
        }

        private void SetInitialState()
        {
            EnsureBackGroundBrush();

            InitialiseDefaultTurtle();

            SelectBackGround(_startBackColorIdx);
            SetBackgroundColor(BackgroundColor);

            DrawGrid();

            SetupTurtleTips();
        }

        private void EnsureBackGroundBrush()
        {
            _drawingSurface.Background = new SolidColorBrush();
        }

        private void MoveTurtle(string turtleName)
        {
            Canvas.SetLeft(_turtlePointers[turtleName],
                           _currentLocations[turtleName].X - (_turtlePointers[turtleName].Width / 2));

            Canvas.SetTop(_turtlePointers[turtleName],
                          _currentLocations[turtleName].Y - (_turtlePointers[turtleName].Height / 2));
        }

        private void SetBackgroundColor(Color backgroundColour)
        {
           ((SolidColorBrush) _drawingSurface.Background).Color = backgroundColour;
        }

        protected virtual void OnCleared(EventArgs e)
        {
            if (Cleared != null)
            {
                Cleared(this, e);
            }
        }

        protected virtual void OnPenWidthChanged(PenWidthEventArgs e)
        {
            if (PenWidthChanged != null)
            {
                PenWidthChanged(this, e);
            }
        }

        protected virtual void OnColorChange(ColorChangeEventArgs e)
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, e);
            }
        }

        protected virtual void OnPenStatusChanged(PenStatusEventArgs e)
        {
            if (PenStatusChanged != null)
            {
                PenStatusChanged(this, e);
            }
        }

        public SilverlightTurtleGraphicsRuntime(ICanceller canceller, Panel drawingSurface, Image turtlePointer, Color[] colors,
            int startForeColorIdx, int startBackColorIdx, TextBox debugWindow, int initialPenWidth, ITurtleViewModel turtleViewModel,
            ScrollViewer screenScroller)
        {
            _canceller = canceller;
            _drawingSurface = drawingSurface;
            _defaultTurtlePointer = turtlePointer;
            _colors = colors;
            _startForeColorIdx = startForeColorIdx;
            _startBackColorIdx = startBackColorIdx;
            _debugWindow = debugWindow;
            _initialPenWidth = initialPenWidth;
            _turtleViewModel = turtleViewModel;
            _screenScroller = screenScroller;
            _drawingSurface.Background = new SolidColorBrush();

            InitialiseStructures();

            SetInitialState();

            SetUpColorConstants();

            SetUpTurtleImageConstants();
        }

        private void SetUpColorConstants()
        {
            FloatVariableHandler.Instance.SetReadOnly(ExtensionMethods.ConvertAll(_colors,
                                                        f => ((WellKnownColors)f.ColorToUInt()).ToString()),
                                                        Enumerable.Range(0, _colors.Length).Select(i => (float)i).ToArray());
        }

        private void SetUpTurtleImageConstants()
        {
            foreach (var turtle in _turtleViewModel.Turtles)
            {
                FloatVariableHandler.Instance.SetReadOnly(turtle.Description, turtle.Id);
            }
        }

        private void SelectPenHelper(int penColor, string turtleName)
        {
            var oldColor = _penColors[turtleName];
            _penColors[turtleName] = ConvertToWpfColor(penColor);

            var colorChangeEventArgs = new ColorChangeEventArgs
            {
                IsForeground = true,
                NewColor = _penColors[turtleName],
                OldColor = oldColor,
                TurtleName = turtleName,
            };

            OnColorChange(colorChangeEventArgs);
        }

        public void SelectPen(int penColor)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                SelectPenHelper(penColor, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => SelectPenHelper(penColor, currentTurtleName));
            }
        }

        private Color ConvertToWpfColor(int penColor)
        {
            return penColor > _colors.Length - 1 ? _penColors[CurrentTurtleName] : _colors[penColor];
        }

        private void SelectBackGroundHelper(int penColor)
        {
            var oldColor = BackgroundColor;
            BackgroundColor = ConvertToWpfColor(penColor);
            SetBackgroundColor(BackgroundColor);

            var colorChangeEventArgs = new ColorChangeEventArgs
            {
                IsForeground = false,
                NewColor = BackgroundColor,
                OldColor = oldColor
            };

            OnColorChange(colorChangeEventArgs);
        }

        public void SelectBackGround(int penColor)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                SelectBackGroundHelper(penColor);
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => SelectBackGroundHelper(penColor));
            }
        }

        public void DeleteTurtleHelper(string turtleName)
        {
            turtleName = turtleName.TrimQuotes().ToLower();

            _drawingSurface.Children.Remove(_turtlePointers[turtleName]);

            _turtlePointers.Remove(turtleName);
            _penWidths.Remove(turtleName);
            _rotationAngles.Remove(turtleName);
            _currentLocations.Remove(turtleName);
            _penUpFlags.Remove(turtleName);
            _penColors.Remove(turtleName);
        }

        public void DeleteTurtle(string turtleName)
        {
            turtleName = turtleName.TrimQuotes().ToLower();

            if (turtleName == DefaultTurtleName)
            {
                throw new TurtleRuntimeException(ResourceHelper.GetStaticText("CannotDeleteDefaultTurtle"));
            }

            EnsureTurtle(turtleName);
  
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                DeleteTurtleHelper(turtleName);
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => DeleteTurtleHelper(turtleName));
            }
        }

        public void ShowTurtle(string turtleName)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                ShowTurtleHelper(turtleName, true);
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => ShowTurtleHelper(turtleName, true));
            }
        }

        private void PenHelper(bool status, string turtleName)
        {
            _penUpFlags[turtleName] = status;

            OnPenStatusChanged(new PenStatusEventArgs
                                   {
                                       IsUp = status,
                                       TurtleName = turtleName
                                   });
        }

        public void PenDown()
        {  
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                PenHelper(false, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => PenHelper(false, currentTurtleName));
            }
         }

        public void PenUp()
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                PenHelper(true, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => PenHelper(true, currentTurtleName));
            }
        }

        public void HideAllTurtlesHelper()
        {
            foreach (var turtleName in _turtlePointers.Keys)
            {
                HideTurtle(turtleName);
            }

            _gridPath.Visibility = Visibility.Collapsed;
        }

        public void HideAllTurtles()
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                HideAllTurtlesHelper();
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(HideAllTurtlesHelper);
            }
        }

        public void ShowAllTurtlesHelper()
        {
            foreach (var turtleName in _turtlePointers.Keys)
            {
                ShowTurtle(turtleName);
            }

            _gridPath.Visibility = Visibility.Visible;    
        }

        public void ShowAllTurtles()
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                ShowAllTurtlesHelper();
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(ShowAllTurtlesHelper);
            }
        }

        public void HideTurtle(string turtleName)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                ShowTurtleHelper(turtleName, false);
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => ShowTurtleHelper(turtleName, false));
            }
        }

        private void DrawingHelper(int length, string turtleName)
        {
            DrawInDirection(length, _rotationAngles[turtleName], turtleName);
            MoveTurtle(turtleName);
        }

        public void Forward(int length)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                DrawingHelper(length, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => DrawingHelper(length, currentTurtleName));
            }
        }

        private void MovementHelper(int length, string turtleName)
        {
            var isPenUp = _penUpFlags[turtleName];

            _penUpFlags[turtleName] = true;
            DrawingHelper(length, turtleName);
            _penUpFlags[turtleName]  = isPenUp;
        }

        public void Move(int length)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                MovementHelper(length, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => MovementHelper(length, currentTurtleName));
            }
        }

        public void Backward(int length)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                DrawingHelper(-length, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => DrawingHelper(-length, currentTurtleName));
            }
        }

        public void Right(int angle)
        { 
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                RotateTurtle(angle, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => RotateTurtle(angle, currentTurtleName));
            }
        }

        private void RotateTurtle(int angle, string turtleName, bool isAbsoluteValue = false)
        {
            if (isAbsoluteValue)
            {
                _rotationAngles[turtleName] = angle;
            }
            else
            {

                _rotationAngles[turtleName] = (_rotationAngles[turtleName] + angle) % GlobalConstants.
                                                                                               ThreeSixtyDegrees;
            }

            _turtlePointers[turtleName].RenderTransform = new RotateTransform
                                                              {
                                                                  Angle = _rotationAngles[turtleName],
                                                                  CenterX =
                                                                      _turtlePointers[turtleName].Width/2,
                                                                  CenterY =
                                                                      _turtlePointers[turtleName].Height/2
                                                              };
        }

        private void ResetHelper()
        {
            SetInitialState();
            OnCleared(EventArgs.Empty);
        }

        public void ClearScreen()
        {
            InitialiseStructures();

            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                ResetHelper();
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(ResetHelper);
            }
        }

        public void DrawSquare(int length)
        {
           if (_penUpFlags[CurrentTurtleName])
           {
               return;
           }

           if (_drawingSurface.Dispatcher.CheckAccess())
           {
               DrawSquareHelper(length, CurrentTurtleName);
           }
           else
           {
               var currentTurtleName = CurrentTurtleName;
               _drawingSurface.Dispatcher.BeginInvoke(() => DrawSquareHelper(length, currentTurtleName));
           }
        }

        public void DrawCircle(int diameter)
        {
            if (_penUpFlags[CurrentTurtleName])
            {
                return;
            }

            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                DrawCircleHelper(diameter, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => DrawCircleHelper(diameter, currentTurtleName));
            }
        }

        public void DrawTriangle(int length)
        {
            if (_penUpFlags[CurrentTurtleName])
            {
                return;
            }

            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                DrawTriangleHelper(length, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => DrawTriangleHelper(length, currentTurtleName));
            }
        }

        private void ShowTurtleHelper(string turtleName, bool showTurtle)
        {
            turtleName = turtleName.TrimQuotes().ToLower();

            EnsureTurtle(turtleName);

            _turtlePointers[turtleName].Visibility = showTurtle ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Rotate(int angle)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                RotateTurtle(angle, CurrentTurtleName, true);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => RotateTurtle(angle, currentTurtleName, true));
            }
        }

        public void Left(int angle)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                RotateTurtle(-angle, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => RotateTurtle(-angle, currentTurtleName));
            }
        }

        private void PenWidthHelper(int width, string currentTurtleName)
        {
            OnPenWidthChanged(new PenWidthEventArgs { Width = width, TurtleName = currentTurtleName });
        }

        public void PenWidth(int width)
        {
            _penWidths[CurrentTurtleName] = width;

            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                PenWidthHelper(width, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => PenWidthHelper(width, currentTurtleName));
            }
        }

        public void CenterTurtle()
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                DrawToCenter(CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => DrawToCenter(currentTurtleName));
            }
        }

        public void Label(string text, float fontSize)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                SetLabelHelper(text, fontSize, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => SetLabelHelper(text, fontSize, currentTurtleName));
            }
        }

        public void Print(string text)
        {
            text = text.TrimQuotes();
 
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                _debugWindow.Text += text;
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => _debugWindow.Text += text);
            }
        }

        public void PrintLine(string text)
        {
            text = text.TrimQuotes() + Environment.NewLine;

            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                _debugWindow.Text += text;
            }
            else
            {
                _drawingSurface.Dispatcher.BeginInvoke(() => _debugWindow.Text += text);
            }
        }

        public void SetX(int x)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                SetXHelper(x, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => SetXHelper(x, currentTurtleName));
            }
        }

        public void SetY(int y)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                SetYHelper(y, CurrentTurtleName);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => SetYHelper(y, currentTurtleName));
            }
        }

        private void SetPosHelper(int x, int y, string turtleName, bool drawLine)
        {
            var originalLocation = _currentLocations[turtleName];
            _currentLocations[turtleName] = new Point(x,y);
     
            MoveTurtle(turtleName);

            if (!_penUpFlags[turtleName] && drawLine)
            {
                DrawLine(originalLocation, _currentLocations[turtleName], turtleName);
            }
        }

        public void SetPos(int x, int y)
        {
            if (_drawingSurface.Dispatcher.CheckAccess())
            {
                SetPosHelper(x,y, CurrentTurtleName, true);
            }
            else
            {
                var currentTurtleName = CurrentTurtleName;
                _drawingSurface.Dispatcher.BeginInvoke(() => SetPosHelper(x, y, currentTurtleName, true));
            }
        }

        private void SetXHelper(int x, string turtleName)
        {
            var originalLocation = _currentLocations[turtleName];
           _currentLocations[turtleName] = new Point(x, _currentLocations[turtleName].Y);

            MoveTurtle(turtleName);

            if (!_penUpFlags[turtleName])
            {
                DrawLine(originalLocation, _currentLocations[turtleName], turtleName);
            }
        }

        private void SetYHelper(int y, string turtleName)
        {
            var originalLocation = _currentLocations[turtleName];
            _currentLocations[turtleName] = new Point(_currentLocations[turtleName].X, y);

            if (!_penUpFlags[turtleName])
            {
                DrawLine(originalLocation, _currentLocations[turtleName], turtleName);
            }
        }

        public void CancelWait()
        {
            _waitCanceller.Set();
        }

        /// <summary>
        /// This method is used to clean-up after a program has finished execution.
        /// </summary>
        public void CleanUpAfterExecution()
        {
            // Reset the current turtle for the specific thread running the program
            CurrentTurtleName = string.Empty;

            SetupTurtleTips();
        }

        public void CenterDefaultTurtle()
        {
            SetToCentre(DefaultTurtleName);
        }

        public void Wait(int delay)
        {
            if (_canceller.ShouldCancel())
            {
                return;
            }

            _waitCanceller.WaitOne(delay);
        }
    }
}
