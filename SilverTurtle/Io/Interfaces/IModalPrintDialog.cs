using System.Windows;

namespace SilverTurtle.Io.Interfaces
{
    public interface IModalPrintDialog
    {
        void PrintPage(string title, UIElement objectToPrint);
    }
}