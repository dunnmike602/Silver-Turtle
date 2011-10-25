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
using System.Windows.Printing;

namespace SilverTurtle.Helpers
{
    public static class PrintHelper
    {
        public static void PrintPage(string title, UIElement objectToPrint)
        {
            var pd = new PrintDocument();

            pd.PrintPage += (s, args) =>
            {
                args.PageVisual = objectToPrint;
            };

            pd.Print(title);
        }
    }
}
