using System.IO;
using SilverTurtle.Helpers;
using SilverTurtle.Io.Interfaces;

namespace SilverTurtle.Io
{
    public class FileOperations : IFileOperations
    {
        public void SaveTextFile(Stream file, string textToSave)
        {
            FileHelper.SaveTextFile(file, textToSave);
        }
    }
}