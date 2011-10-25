using System.Windows;
using SilverTurtle.Helpers;
using SilverTurtle.Io.Interfaces;

namespace SilverTurtle.Io
{
    public class SilverlightModalPrintDialog : IModalPrintDialog
    {
        public void PrintPage(string title, UIElement objectToPrint)
        {
            PrintHelper.PrintPage(ResourceHelper.GetStaticText("PrintHeading"), objectToPrint);
        }
    }
}