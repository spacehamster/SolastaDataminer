using Newtonsoft.Json;
using System;

namespace Dataminer
{
    public class DefinitionConverter : JsonConverter
    {
        public bool CannotWrite;
        public override bool CanWrite
        {
            get
            {
                return !CannotWrite;
            }
        }
        public bool CannotRead;
        public override bool CanRead
        {
            get
            {
                return !CannotRead;
            }
        }
        public DefinitionConverter() { }

        public override void WriteJson(JsonWriter w, object o, JsonSerializer szr)
        {
            using (new PushValue<bool>(true, () => CannotWrite, (canWrite) => CannotWrite = canWrite))
            {
                szr.Serialize(w, o);
            }
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer szr)
        {
            throw new NotImplementedException();
        }
        public override bool CanConvert(Type type)
        {
            return typeof(BaseDefinition).IsAssignableFrom(type);
        }
    }
}
