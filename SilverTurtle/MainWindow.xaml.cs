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

using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using SilverTurtle.Helpers;
using SilverTurtle.IOC.Interfaces;
using SilverTurtle.Models;
using SilverTurtle.ViewModels;
using TurtleGraphics.Enums;
using TurtleGraphics.Interfaces;

namespace SilverTurtle
{
    public partial class MainWindow : IInjectable
    {
        private Color[] _colors;
        private MainPageViewModel _mvvm;
        private const int StartForeColorIdx = 7;
        private const int StartBackColorIdx = 1;
        private const int InitialPenWidth = 1;

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public MainWindow()
        {
            InitializeComponent();
  }

        private void SetupColors()
        {
            const WellKnownColors wellKnownColors = WellKnownColors.Turquoise;

            _colors = ExtensionMethods.ConvertAll((uint[])wellKnownColors.GetValues(),
                                                  f => f.ToColor());
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            SetupColors();
            ResolveDependancies();

            _mvvm = (MainPageViewModel) DataContext;

            _mvvm.InitialiseTurtleGraphicsSystem();
            _mvvm.StartTurtleGraphicsSystem();
        }

        private void ResolveDependancies()
        {
            var parameterOverrides = new ParameterOverrides
                                         {
                                             {"drawingSurface", turtleCanvas},
                                             {"turtlePointer", imgTurtle},
                                             {"colors", _colors},
                                             {"startForeColorIdx", StartForeColorIdx},
                                             {"startBackColorIdx", StartBackColorIdx},
                                             {"debugWindow", DebugWindow},
                                             {"initialPenWidth", InitialPenWidth},
                                             {"screenScroller", ScreenScroller},
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
                                             {"turtlePointer", imgTurtle},
                                             {"debugWindow", DebugWindow},
                                             {"tiErrorTab", tiErrors},
                                             {"turtleGraphicsRuntime", wpfTurtleGraphics},
                                             {"startForeColorIndex", StartForeColorIdx},
                                             {"initialPenWidth", InitialPenWidth},
                                             {"colors", _colors},
                                         };

            var mvvm = UnityContainer.Resolve<MainPageViewModel>("MAINPAGEVIEWMODEL", parameterOverrides);

            DataContext = mvvm;

        }

        private void MainWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _mvvm = (MainPageViewModel)DataContext;
            _mvvm.CentreDefaultTurtle();
        }
    }
}
    