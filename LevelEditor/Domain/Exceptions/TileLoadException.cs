using System;

namespace LevelEditor.Domain.Exceptions
{
    [Serializable]
    public class TileLoadException : Exception {
        public TileLoadException(string s) : base(s) {

        }
    }
}