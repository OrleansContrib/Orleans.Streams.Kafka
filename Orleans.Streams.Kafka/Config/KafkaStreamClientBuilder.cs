using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Streams.Utils.Serialization;
using System;

namespace Orleans.Streams.Kafka.Config;

public class KafkaStreamClientBuilder
{
	private readonly IClientBuilder _hostBuilder;
	private readonly string _providerName;
	private Action<KafkaStreamOptions> _configure;

	public KafkaStreamClientBuilder(IClientBuilder hostBuilder, string providerName)
	{
		_hostBuilder = hostBuilder;
		_providerName = providerName;
	}

	public KafkaStreamClientBuilder WithOptions(Action<KafkaStreamOptions> configure)
	{
		_configure = configure;
		return this;
	}

	public KafkaStreamClientBuilder AddExternalDeserializer<TDeserializer>()
		where TDeserializer : class, IExternalStreamDeserializer
	{
		_hostBuilder.ConfigureServices(services
			=> services.AddSingletonNamedService<IExternalStreamDeserializer, TDeserializer>(_providerName)
		);

		return this;
	}

	public KafkaStreamClientBuilder AddAvro(string schemaRegistryUrl)
	{
		_hostBuilder.AddAvro(_providerName, schemaRegistryUrl);
		return this;
	}

	public KafkaStreamClientBuilder AddJson()
	{
		_hostBuilder.AddJson(_providerName);
		return this;
	}

	public IClientBuilder Build()
	{
		_hostBuilder.AddKafkaStreamProvider(
			_providerName,
			options => _configure?.Invoke(options)
		);

		return _hostBuilder;
	}
}