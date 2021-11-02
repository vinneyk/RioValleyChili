namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class AdditiveTypeDto 
    {
        public string Key { get; set; }
        public string Description { get; set; }

        #region equality overrides

        protected bool Equals(AdditiveTypeDto other)
        {
            return string.Equals(Key, other.Key) && string.Equals(Description, other.Description);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0)*397) ^
                       (Description != null ? Description.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AdditiveTypeDto) obj);
        }

        #endregion

    }
}