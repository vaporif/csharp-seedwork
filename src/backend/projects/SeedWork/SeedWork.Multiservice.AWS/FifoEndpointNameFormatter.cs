namespace SeedWork.Multiservice.Services;

using System;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Saga;

public class FifoEndpointNameFormatter : IEndpointNameFormatter
{
    private readonly IEndpointNameFormatter _formatter;
    private readonly string _prefix;

    public FifoEndpointNameFormatter(IEndpointNameFormatter formatter, string prefix = null)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _prefix = prefix;
    }

    public string Separator => "_";

    public string TemporaryEndpoint(string tag) => _formatter.TemporaryEndpoint(tag);

    public string Consumer<T>()
        where T : class, IConsumer =>
        SanitizeName(AddPrefix(_formatter.Consumer<T>()));

    public string Message<T>()
        where T : class =>
        SanitizeName(AddPrefix(_formatter.Message<T>()));

    public string Saga<T>()
        where T : class, ISaga =>
        SanitizeName(AddPrefix(_formatter.Saga<T>()));

    public string ExecuteActivity<T, TArguments>()
        where T : class, IExecuteActivity<TArguments>
        where TArguments : class =>
        SanitizeName(AddPrefix(_formatter.ExecuteActivity<T, TArguments>()));

    public string CompensateActivity<T, TLog>()
        where T : class, ICompensateActivity<TLog>
        where TLog : class =>
        SanitizeName(AddPrefix(_formatter.CompensateActivity<T, TLog>()));

    public string SanitizeName(string name) => _formatter.SanitizeName(name);

    private string AddPrefix(string name) => $"{(string.IsNullOrWhiteSpace(_prefix) ? name : $"{_prefix}_{name}")}.fifo";
}
