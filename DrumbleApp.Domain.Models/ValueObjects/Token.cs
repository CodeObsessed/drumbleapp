using System;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class Token
    {
        public Guid Value { get; private set; }

        public Token(Guid value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
