using System;

namespace SocialMedia.Helpers.Interfaces
{
	public interface ISerializationHelper
	{
		T DeserializeObjectAsync<T>(String metaData);
		String SerializeObjectAsync<T>(T adminObject);
	}
}