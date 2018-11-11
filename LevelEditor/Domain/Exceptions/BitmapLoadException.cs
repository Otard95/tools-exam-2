using System;

namespace LevelEditor.Domain.Exceptions
{
    [Serializable]
    public class BitmapLoadException : Exception
    {
        public BitmapLoadException(string s, Exception exception) : base(s, exception)
        {

        }
    }
}