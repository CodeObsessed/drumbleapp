using System.Collections.ObjectModel;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class Country
    {
        public string Code { get; private set; }
        public string Name { get; private set; }

        public Country(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }
    }
}
