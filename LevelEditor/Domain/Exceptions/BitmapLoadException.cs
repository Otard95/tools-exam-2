using System;

namespace LevelEditor.Services
{
    [Serializable]
    public class BitmapLoadException : Exception
    {
        public BitmapLoadException(string s, Exception exception) : base(s, exception)
        {

        }
    }
}