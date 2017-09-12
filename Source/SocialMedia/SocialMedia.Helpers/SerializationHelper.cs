using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocialMedia.Helpers.Interfaces;

namespace SocialMedia.Helpers
{
	public class SerializationHelper : ISerializationHelper
	{
		public T DeserializeObjectAsync<T>(String metaData)
		{
			return JsonConvert.DeserializeObject<T>(
				value: metaData,
				settings: new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects,
					ObjectCreationHandling = ObjectCreationHandling.Replace
				}
			);
		}

		public String SerializeObjectAsync<T>(T obj)
		{
			return JsonConvert.SerializeObject(
				value: obj,
				formatting: Formatting.None,
				settings: new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects,
					TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
				}
			);
		}
	}
}
