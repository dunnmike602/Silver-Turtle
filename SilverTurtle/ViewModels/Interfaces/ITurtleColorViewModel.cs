using System.Collections.Generic;
using SilverTurtle.Models;

namespace SilverTurtle.ViewModels.Interfaces
{
    public interface ITurtleColorViewModel
    {
        List<TurtleColor> TurtleColors { get; }
    }
}