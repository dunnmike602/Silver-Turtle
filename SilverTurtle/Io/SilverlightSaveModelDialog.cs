using System.IO;
using System.Windows.Controls;
using SilverTurtle.Io.Interfaces;

namespace SilverTurtle.Io
{
    public class SilverlightSaveModelDialog : IModalSaveDialog
    {
        private readonly SaveFileDialog _saveFileDialog;

        public SilverlightSaveModelDialog()
        {
            _saveFileDialog = new SaveFileDialog
            {
                Filter = "Turtle Pictures (.tur)|*.tur",
            };   
        }

        public bool? DialogResult
        {
            get; set;
        }

        public void Show()
        {
            DialogResult = _saveFileDialog.ShowDialog();
        }

        public string SafeFileName()
        {
            return _saveFileDialog.SafeFileName;
        }

        public Stream File()
        {
            return _saveFileDialog.OpenFile();
        }
    }
}