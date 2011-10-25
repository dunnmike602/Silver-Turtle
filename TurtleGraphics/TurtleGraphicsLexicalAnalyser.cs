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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TurtleGraphics.Constants;
using TurtleGraphics.Interfaces;

namespace TurtleGraphics
{
    public class TurtleGraphicsLexicalAnalyser : ITurtleGraphicsLexicalAnalyser
    {
        public List<string> Parse(string text)
        {
            var regexPattern = new Regex(GlobalConstants.LexerPattern);
            var matches = regexPattern.Matches(text);

            var tokens = new List<string>();

            for (var nextMatch = 0; nextMatch < matches.Count; nextMatch++)
            {
                if (!string.IsNullOrEmpty(matches[nextMatch].Value))
                {
                    tokens.Add(matches[nextMatch].Value);
                }
            }

            return tokens;
        }
    }
}
