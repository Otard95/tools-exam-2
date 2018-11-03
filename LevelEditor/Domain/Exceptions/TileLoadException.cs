using System;

namespace LevelEditor.Domain.Exceptions
{
    public class TileLoadException : Exception {
        public TileLoadException(string s) : base(s) {

        }
    }
}