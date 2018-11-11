using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LevelEditor.Domain.Exceptions;
using Microsoft.Win32;

namespace LevelEditor.Services {
    public static class FileService {

        public static void OpenFile<T>(string defaultName, string extension, Action<T, string> loadedFileObjectCallback)
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

            T fileAsLoadedObject;
            try {
                fileAsLoadedObject = JsonService.LoadGet<T>(dialog.FileName, fullPathProvided: true);
            }
            catch (Exception e) {
                // throw new FileServiceException($"Could not load file from {dialog.FileName}", e);
                MessageBox.Show($"Could not load file from {dialog.FileName}: {e.InnerException?.Message}", "FileService returned an Error", MessageBoxButton.OK);
                return;
            }
            loadedFileObjectCallback.Invoke(fileAsLoadedObject, dialog.FileName);
        }

        public static void SaveFileAs<T>(T objectToSave, string defaultName, string extension, Action<string> savedFileNameCallback = null) {
            var extensionLowerCase = extension.ToLower();
            var extensionUpperCase = extension.ToUpper();
            var dialog = new SaveFileDialog {
                FileName = defaultName,
                DefaultExt = $"*.{extensionLowerCase}",
                Filter = $"{extensionUpperCase}|*.{extensionLowerCase}"
            };
            var result = dialog.ShowDialog();
            if (result != true) return;
            SaveFile(objectToSave, dialog.FileName);
            savedFileNameCallback?.Invoke(dialog.FileName);
        }

        public static void SaveFile<T>(T objectToSave, string fullFilePath) {
            if (!JsonService.Save(objectToSave, "", filename: fullFilePath, fullPathProvided: true))
                throw new FileServiceException($"Could not save file to {fullFilePath}");
        }
    }
}
