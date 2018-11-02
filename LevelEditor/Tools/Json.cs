using System;
using System.IO;
using Newtonsoft.Json;

namespace LevelEditor.Tools {
    public static class Json {
        /// <summary>
        /// Serializes content to Json and saves to .json file
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized</typeparam>
        /// <param name="content">Content to save</param>
        /// <param name="folder">Folder where the file should be located</param>
        /// <param name="filename">name of the file without type specifier. .enc is added automatically</param>
        /// <param name="fullPathProvided">Whether full system path is provided or to look in current working directory</param>
        /// <returns></returns>
        public static bool Save<T>(T content, string folder, string filename, bool fullPathProvided = false) {
            if (filename == "New") return false; //Never overwrite default

            try {
                var json = JsonConvert.SerializeObject(content, Formatting.Indented);
                if (!fullPathProvided) {
                    var rootPath = Directory.GetCurrentDirectory();
                    var path = $"{rootPath}/{folder}";
                    Directory.CreateDirectory(path);
                    File.WriteAllText($"{path}/{filename}.json", json);
                }
                else {
                    File.WriteAllText(filename, json);
                }

                return true;
            }
            catch (Exception) {
                //Debug.Instance.Log(e.Message);
            }
            return false;
        }


        /// <summary>
        /// Loads a Json-formatted file and tries to serialize it to given type
        /// </summary>
        /// <typeparam name="T">Type to be serialized to</typeparam>
        /// <param name="content">Pointer to object where loaded content will be stored</param>
        /// <param name="filename">Name of file to load</param>
        /// <param name="folder">Name of folder to load from</param>
        /// <param name="fullPathProvided">Whether full system path is provided or to look in current working directory</param>
        /// <returns></returns>
        public static bool Load<T>(ref T content, string filename, string folder = "", bool fullPathProvided = false) {
            try {
                var rootPath = Directory.GetCurrentDirectory();
                var json = File.ReadAllText(fullPathProvided ? filename : $"{rootPath}/{(string.IsNullOrEmpty(folder) ? "" : $"{folder}/")}{filename}.json");
                content = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception) {
                //Debug.Instance.Log(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Loads a Json-formatted file and tries to serialize it to given type, and returns the result as the serialized object.
        /// Will throw if there are any problems with the file or serialization
        /// </summary>
        /// <typeparam name="T">Type to be serialized to</typeparam>
        /// <param name="filename">Name of file to load</param>
        /// <param name="folder">Name of folder to load from</param>
        /// <param name="fullPathProvided">Whether full system path is provided or to look in current working directory</param>
        /// <returns></returns>
        public static T LoadGet<T>(string filename, string folder = "", bool fullPathProvided = false) {
            var rootPath = Directory.GetCurrentDirectory();
            var json = File.ReadAllText(fullPathProvided ? filename : $"{rootPath}/{(string.IsNullOrEmpty(folder) ? $"{folder}/" : "")}{filename}.json");
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}