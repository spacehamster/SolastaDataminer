using System;
using Newtonsoft.Json;

namespace Dataminer
{
    public class DefinitionReferenceConverter : JsonConverter
    {
        public DefinitionReferenceConverter() { }

        public override void WriteJson(JsonWriter w, object o, JsonSerializer szr)
        {
            try
            {
                var def = (BaseDefinition)o;
                w.WriteValue(string.Format($"Definition:{def.Name}:{def.GUID}"));
            }
            catch (InvalidCastException ex)
            {
                w.WriteValue(string.Format($"Error:{o?.GetType().FullName ?? "NULL"}:{ex.ToString()}"));
            }
        }
        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer
        )
        {
            throw new NotImplementedException();
        }
        private static readonly Type _tBaseDefinition = typeof(BaseDefinition);
        public override bool CanConvert(Type type) => _tBaseDefinition.IsAssignableFrom(type);
    }
}