using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

namespace Dataminer
{
    public class JsonUtil
    {
        public static JsonSerializerSettings CreateSettings()
        {
            var RefJsonSerializerSettings = new JsonSerializerSettings
            {
                CheckAdditionalContent = false,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new DataminerConstractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Double,
                Formatting = Formatting.Indented,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                StringEscapeHandling = StringEscapeHandling.Default,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Objects
            };
            return RefJsonSerializerSettings;
        }
        public static void Dump(BaseDefinition definition, string path, JsonSerializerSettings settings = null)
        {
            var folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);
            if (settings == null) settings = CreateSettings();
            JsonSerializer serializer = JsonSerializer.Create(settings);
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, definition);
            }
        }

        public static bool IsBlacklisted(MemberInfo memberInfo)
        {
            return false;
        }

        public static List<MemberInfo> GetUnitySerializableMembers(Type objectType)
        {

            if (objectType == null)
                return new List<MemberInfo>();
            IEnumerable<MemberInfo> publicFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(f => !f.IsInitOnly);
            IEnumerable<MemberInfo> privateFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            IEnumerable<MemberInfo> newtonsoftFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(f => (f.IsPublic && f.IsInitOnly || f.IsPrivate) && Attribute.IsDefined(f, typeof(JsonPropertyAttribute)));
            IEnumerable<MemberInfo> newtonsoftProperties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(p => Attribute.IsDefined(p, typeof(JsonPropertyAttribute)));
            IEnumerable<MemberInfo> nameProperty = objectType == typeof(UnityEngine.Object) ?
                    new MemberInfo[] { objectType.GetProperty("name") } :
                    Array.Empty<MemberInfo>();
            var result = privateFields
                .Where((field) => Attribute.IsDefined(field, typeof(SerializeField)))
                .Concat(publicFields)
                .Concat(GetUnitySerializableMembers(objectType.BaseType))
                .Concat(nameProperty)
                .Concat(newtonsoftProperties)
                .Concat(newtonsoftFields)
                .Where(field => !IsBlacklisted(field))
                .ToList();
            return result;
        }
    }
}
