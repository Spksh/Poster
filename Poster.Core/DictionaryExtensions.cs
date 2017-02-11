using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Poster.Core
{
    public static class DictionaryExtensions
    {
        // TODO: Make this typesafe; it won't work for generic objects (no reflection) and we only use it for deserialized YAML
        /// <summary>
        /// Converts YamlDotNet deserialized Dictionary<object, object> to a dynamic ExpandoObject we can use in scripts
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDynamic(this object value)
        {
            if (value == null)
            {
                return null;
            }

            IList container = value as IList;

            if (container != null && container.Count > 0)
            {
                List<dynamic> members = new List<dynamic>(container.Count);

                foreach (object member in container)
                {
                    members.Add(member.ToDynamic());
                }

                return members;
            }

            IDictionary type = value as IDictionary;

            if (type != null)
            {
                IDictionary<string, dynamic> member = new ExpandoObject();

                foreach (string key in type.Keys)
                {
                    member[key] = type[key].ToDynamic();
                }

                return (ExpandoObject)member;
            }

            return value;
        }
    }
}
