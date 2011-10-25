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
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace SilverTurtle.Helpers
{
    public static class ResourceHelper
    {
        private const string SnippitResourceFileName = "SilverTurtle.Resources.Snippits";
        private const string StaticTextResourceFileName = "SilverTurtle.Resources.StaticText";

        private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

        public static string GetCodeSnippit(string snippitName)
        {
            var resources = new ResourceManager(SnippitResourceFileName, CurrentAssembly);

            return resources.GetString(snippitName);
        }

        public static string GetStaticText(string staticTextName)
        {
            var resources = new ResourceManager(StaticTextResourceFileName, CurrentAssembly);

            return resources.GetString(staticTextName);
        }

        public static Stream GetApplicationResourceStream(string resourceUriString)
        {
            return Application.GetResourceStream(new Uri(resourceUriString, UriKind.Relative)).Stream;
        }
    }
}
