using System;

namespace LevelEditor.Services
{
    public class BitmapLoadException : Exception
    {
        public BitmapLoadException(string s, Exception exception) : base(s, exception)
        {

        }
    }
}