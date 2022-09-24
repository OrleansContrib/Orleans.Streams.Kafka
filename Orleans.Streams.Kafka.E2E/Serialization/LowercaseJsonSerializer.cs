using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Orleans.Streams.Kafka.E2E.Serialization;

public class LowercaseJsonSerializer<T> : ISerializer<T>
{
	private static readonly JsonSerializerSettings _settings = new()
    {
		ContractResolver = new LowercaseContractResolver()
	};

	public class LowercaseContractResolver : DefaultContractResolver
	{
		protected override string ResolvePropertyName(string propertyName)
			=> propertyName.Equals("minOffset") || propertyName.Equals("maxOffset")
				? propertyName
				: propertyName.ToLower();
	}

	public byte[] Serialize(T data, SerializationContext context)
		=> Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.None, _settings));
}