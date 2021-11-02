using Newtonsoft.Json;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public abstract class Serializable
    {
        public static TDest Deserialize<TDest>(string serialized)
            where TDest : Serializable
        {
            return string.IsNullOrWhiteSpace(serialized) ? null : JsonConvert.DeserializeObject<TDest>(serialized);
        }

        public static implicit operator string(Serializable serializable)
        {
            return serializable == null ? null : serializable.Serialize();
        }

        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        private Serializable() { }

        public abstract class Base<TSource> : Serializable
            where TSource : class
        {
            public override string ToString()
            {
                return Serialize();
            }

            protected Base(TSource source)
            {
                InitializeFromSource(source);
            }

            protected abstract void InitializeFromSource(TSource source);
        }
    }
}