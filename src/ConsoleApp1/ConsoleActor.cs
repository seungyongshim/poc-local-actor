using Proto;

public interface IMessageId
{
    public string Id { get; init; }
};

public record MessageId(string Id) : IMessageId
{
}

public class ProxyActor : IActor
{

    public ProxyActor(Props propsChild)
    {
        PropsChild = propsChild;
    }

    public Props PropsChild { get; }

    public Task ReceiveAsync(IContext context) => context.Message switch
    {
        IMessageId v => Task.Factory.StartNew(() => 
        {
            var pid = PID.FromAddress(context.Self.Address, $"{context.Self.Id}/{v.Id}");

            if (!context.Children.Contains(pid))
            {
                var p = context.SpawnNamed(PropsChild, v.Id);
                context.Forward(p);
            }
            else
            {
                context.Forward(pid);
            }

            
        }),
        _ => Task.CompletedTask
    };

}

public class ConsoleActor : IActor
{
    public Task ReceiveAsync(IContext context) => context.Message switch
    {
        { } msg => Task.Factory.StartNew(() => Console.WriteLine(msg))
    };
}
