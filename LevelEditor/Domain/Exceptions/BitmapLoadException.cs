using System;

namespace LevelEditor.Domain.Exceptions
{
    public class BitmapLoadException : Exception
    {
        public BitmapLoadException(string s, Exception exception) : base(s, exception)
        {

        }
    }
}