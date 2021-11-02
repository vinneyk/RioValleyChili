namespace RioValleyChili.Services.Utilities.Models
{
    internal class AttributeNameAndType
    {
        internal string Key { get; set; }

        internal string Name { get; set; }

        internal bool ValidForChile { get; set; }

        internal bool ValidForAdditive { get; set; }

        internal bool ValidForPackaging { get; set; }
    }
}