using Medallion.Threading.Redis;
using Proto;
using StackExchange.Redis;

public class ConsoleActor : IActor, IAsyncDisposable
{
    public ConsoleActor(IDatabase connection)
    {
        Database = connection;
    }

    public IDatabase Database { get; }
    public RedisDistributedLockHandle? Handle { get; private set; }

    public async ValueTask DisposeAsync()
    {
        var t = Handle?.DisposeAsync() ?? ValueTask.CompletedTask;
        await t;
    }

    public async Task ReceiveAsync(IContext context) 
    {
        var task = context.Message switch
        {
            Started => Task.Factory.StartNew(async () =>
            {
                var @lock = new RedisDistributedLock(context.Self.ToString(), Database);
                Handle = await @lock.AcquireAsync(TimeSpan.FromSeconds(3));

                context.SetReceiveTimeout(TimeSpan.FromSeconds(3));
            }),
            ReceiveTimeout => Task.Factory.StartNew(() => context.Send(context.Parent!, new ProxyPoisonTarget(context.Self))),
            { } v => Task.Factory.StartNew(() => Console.WriteLine(v)),
            _ => Task.CompletedTask
        };

        await task;
    }
}
