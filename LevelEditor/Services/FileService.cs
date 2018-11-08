using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LevelEditor.Services {
    public static class FileService {
        public static void OpenFile<T>(string defaultName, string extension, Action<T> loadedFileObjectCallback)
        {
            var extensionLowerCase = extension.ToLower();
            var extensionUpperCase = extension.ToUpper();
            var dialog = new OpenFileDialog {
                FileName = defaultName,
                DefaultExt = $"*.{extensionLowerCase}",
                Filter = $"{extensionUpperCase}|*.{extensionLowerCase}"
            };
            var result = dialog.ShowDialog();
            if (result != true) return;
            try {
                var fileAsLoadedObject = JsonService.LoadGet<T>(dialog.FileName, fullPathProvided: true);
                loadedFileObjectCallback.Invoke(fileAsLoadedObject);
            }
            catch (Exception) {
                //Display corrupted file error
            }
        }

        public static void SaveFile<T>(T objectToSave, string defaultName, string extension) {
            var extensionLowerCase = extension.ToLower();
            var extensionUpperCase = extension.ToUpper();
            var dialog = new SaveFileDialog {
                FileName = defaultName,
                DefaultExt = $"*.{extensionLowerCase}",
                Filter = $"{extensionUpperCase}|*.{extensionLowerCase}"
            };
            var result = dialog.ShowDialog();
            if (result != true) return;
            JsonService.Save(objectToSave, "", filename: dialog.FileName, fullPathProvided: true);
        }
    }
}
