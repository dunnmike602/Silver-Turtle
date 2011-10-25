using System.IO;

namespace SilverTurtle.Io.Interfaces
{
    public interface IModalLoadDialog
    {
        bool? DialogResult { get; set; }
        void Show();
        StreamReader FileContents();
    }
}