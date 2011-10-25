using System.Collections.Generic;
using SilverTurtle.Models;

namespace SilverTurtle.ViewModels.Interfaces
{
    public interface ITurtleMethodViewModel
    {
        List<TurtleMethod> TurtleMethods { get; }
    }
}