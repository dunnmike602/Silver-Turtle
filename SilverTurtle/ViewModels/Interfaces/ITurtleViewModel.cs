using System.Collections.Generic;
using SilverTurtle.Models;

namespace SilverTurtle.ViewModels.Interfaces
{
    public interface ITurtleViewModel
    {
        List<Turtle> Turtles { get; }
    }
}