using Proto;

public class ConsoleActor : IActor
{
    public Task ReceiveAsync(IContext context) => context.Message switch
    {
        Started => Task.Factory.StartNew(() => context.SetReceiveTimeout(TimeSpan.FromSeconds(3))),
        ReceiveTimeout => Task.Factory.StartNew(() => context.Send(context.Parent, new ProxyPoisonTarget(context.Self))),
        { } msg => Task.Factory.StartNew(() => Console.WriteLine(msg))
    };
}
