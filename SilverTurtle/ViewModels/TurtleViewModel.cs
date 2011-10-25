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
using SilverTurtle.Models;
using SilverTurtle.ViewModels.Interfaces;
using SimpleMvvmToolkit;

namespace SilverTurtle.ViewModels
{
    public class TurtleViewModel : ViewModelDetailBase<TurtleViewModel, Turtle>, ITurtleViewModel
    {
        private readonly List<Turtle> _turtles = new List<Turtle>();

        private void Populate()
        {
            _turtles.Add(new Turtle { Id = 0, Image = "/SilverTurtle;component/Images/turtle.png", Description = "Turtle" });
            _turtles.Add(new Turtle { Id = 1, Image = "/SilverTurtle;component/Images/unicorn.png", Description = "Unicorn" });
            _turtles.Add(new Turtle { Id = 2, Image = "/SilverTurtle;component/Images/rabbit.png", Description = "Rabbit" });
            _turtles.Add(new Turtle { Id = 3, Image = "/SilverTurtle;component/Images/shark.png", Description = "Shark" });
            _turtles.Add(new Turtle { Id = 4, Image = "/SilverTurtle;component/Images/ant.png", Description = "Ant" });
            _turtles.Add(new Turtle { Id = 5, Image = "/SilverTurtle;component/Images/bee.png", Description = "Bee" });
            _turtles.Add(new Turtle { Id = 6, Image = "/SilverTurtle;component/Images/dog.png", Description = "Dog" });
            _turtles.Add(new Turtle { Id = 7, Image = "/SilverTurtle;component/Images/pig.png", Description = "Pig" });
            _turtles.Add(new Turtle { Id = 8, Image = "/SilverTurtle;component/Images/fox.png", Description = "Fox" });
            _turtles.Add(new Turtle { Id = 9, Image = "/SilverTurtle;component/Images/cat.png", Description = "Cat" });
            _turtles.Add(new Turtle { Id = 10, Image = "/SilverTurtle;component/Images/owl.png", Description = "Owl" });
        }

        public TurtleViewModel()
        {
            Populate();
        }

        public List<Turtle> Turtles
        {
            get { return _turtles; }
        }
    }
}