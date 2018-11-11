using System;

namespace LevelEditor.Domain.Exceptions
{
    public class TileRuleException : Exception
    {
        public TileRuleException(string s) : base(s)
        {
        }
    }
}