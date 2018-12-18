using System;

namespace LevelEditor.Domain.Exceptions
{
    [Serializable]
    public class FileServiceException : Exception
    {
        public FileServiceException(string s) : base (s)
        {
        }

        public FileServiceException(string s, Exception e) : base(message: s, innerException: e) {
        }
    }
}