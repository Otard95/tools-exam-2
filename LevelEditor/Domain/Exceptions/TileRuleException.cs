using System;

namespace LevelEditor.Domain.Exceptions
{
    [Serializable]
    public class TileRuleException : Exception
    {
        public TileRuleException(string s) : base(s)
        {
        }
    }
}