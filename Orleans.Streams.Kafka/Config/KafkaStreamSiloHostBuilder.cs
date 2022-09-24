using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Streams.Kafka.Consumer;
using Orleans.Streams.Utils.MessageTracking;
using Orleans.Streams.Utils.Serialization;
using System;

namespace Orleans.Streams.Kafka.Config;

public class KafkaStreamSiloHostBuilder
{
	private readonly ISiloHostBuilder _hostBuilder;
	private readonly string _providerName;
	private Action<KafkaStreamOptions> _configure;

	public KafkaStreamSiloHostBuilder(ISiloHostBuilder hostBuilder, string providerName)
	{
		_hostBuilder = hostBuilder;
		_providerName = providerName;
	}

	public KafkaStreamSiloHostBuilder WithOptions(Action<KafkaStreamOptions> configure)
	{
		_configure = configure;
		return this;
	}

	public KafkaStreamSiloHostBuilder AddExternalDeserializer<TDeserializer>()
		where TDeserializer : class, IExternalStreamDeserializer
	{
		_hostBuilder.ConfigureServices(services
			=> services.AddSingletonNamedService<IExternalStreamDeserializer, TDeserializer>(_providerName)
		);

		return this;
	}

    public KafkaStreamSiloHostBuilder AddStreamIdSelector<TSelector>()
        where TSelector : class, IStreamIdSelector
    {
        _hostBuilder.ConfigureServices(services
            => services.AddSingletonNamedService<IStreamIdSelector, TSelector>(_providerName)
        );

        return this;
    }

    public KafkaStreamSiloHostBuilder AddStreamIdSelector<TSelector>(Func<IServiceProvider, string, TSelector> configure)
        where TSelector : class, IStreamIdSelector
    {
        _hostBuilder.ConfigureServices(services
            => services.AddSingletonNamedService<TSelector>(
                _providerName,
                (provider, name) => configure?.Invoke(provider, name))
        );

        return this;
    }

	public KafkaStreamSiloHostBuilder AddAvro(string schemaRegistryUrl)
	{
		_hostBuilder.AddAvro(_providerName, schemaRegistryUrl);
		return this;
	}

	public KafkaStreamSiloHostBuilder AddJson()
	{
		_hostBuilder.AddJson(_providerName);
		return this;
	}

	public KafkaStreamSiloHostBuilder AddMessageTracking<TTraceWriter>()
		where TTraceWriter : class, ITraceWriter
	{
		_hostBuilder.ConfigureServices(services
			=> services.AddSingletonNamedService<ITraceWriter, TTraceWriter>(_providerName)
		);

		return this;
	}

	public KafkaStreamSiloHostBuilder AddMessageTracking<TTraceWriter>(Func<IServiceProvider, string, TTraceWriter> configure)
		where TTraceWriter : class, ITraceWriter
	{
		_hostBuilder.ConfigureServices(services
			=> services.AddSingletonNamedService<ITraceWriter>(
				_providerName,
				(provider, name) => configure?.Invoke(provider, name))
		);


		return this;
	}

	public KafkaStreamSiloHostBuilder AddLoggingTracker()
	{
		_hostBuilder.UseLoggingTracker(_providerName);
		return this;
	}

	public ISiloHostBuilder Build()
	{
		_hostBuilder.AddKafkaStreamProvider(
			_providerName,
			options => _configure?.Invoke(options)
		);

		return _hostBuilder;
	}
}