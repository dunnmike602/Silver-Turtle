using System.IO;
using System.Windows.Controls;
using SilverTurtle.Io.Interfaces;

namespace SilverTurtle.Io
{
    public class SilverlightLoadModelDialog : IModalLoadDialog
    {
        private readonly OpenFileDialog _loadFileDialog;

        public SilverlightLoadModelDialog()
        {
            _loadFileDialog = new OpenFileDialog
            {
                Filter = "Turtle Pictures (.tur)|*.tur",
            };  
        }

        public bool? DialogResult
        {
            get;
            set;
        }

        public void Show()
        {
            DialogResult = _loadFileDialog.ShowDialog();
        }

        public StreamReader FileContents()
        {
            return _loadFileDialog.File.OpenText();
        }
    }
}