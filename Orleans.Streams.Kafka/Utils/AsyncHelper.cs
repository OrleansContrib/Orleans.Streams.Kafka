﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.Streams.Kafka.Utils;

public static class AsyncHelper
{
	private static readonly TaskFactory _myTaskFactory = new(
		CancellationToken.None,
		TaskCreationOptions.None,
		TaskContinuationOptions.None,
		TaskScheduler.Default
	);

	public static TResult RunSync<TResult>(Func<Task<TResult>> func)
	{
		var cultureUi = CultureInfo.CurrentUICulture;
		var culture = CultureInfo.CurrentCulture;
		return _myTaskFactory.StartNew(() =>
		{
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = cultureUi;
			return func();
		}).Unwrap().GetAwaiter().GetResult();
	}

	public static void RunSync(Func<Task> func)
	{
		var cultureUi = CultureInfo.CurrentUICulture;
		var culture = CultureInfo.CurrentCulture;
		_myTaskFactory.StartNew(() =>
		{
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = cultureUi;
			return func();
		}).Unwrap().GetAwaiter().GetResult();
	}
}