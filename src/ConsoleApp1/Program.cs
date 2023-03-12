using Medallion.Threading.Redis;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Logging;
using StackExchange.Redis;





var connection = await ConnectionMultiplexer.ConnectAsync("localhost"); // uses StackExchange.Redis
var dbRedis = connection.GetDatabase();

var system = new ActorSystem(new ActorSystemConfig
{
    DiagnosticsLogLevel = Microsoft.Extensions.Logging.LogLevel.Trace
});

var root = system.Root;

var props = Props.FromProducer(() =>
                new ProxyActor(Props.FromProducer(() =>
                    new ProxyActor(Props.FromProducer(() =>
                        new ConsoleActor(dbRedis))))));

var pid = root.SpawnNamed(props, nameof(ProxyActor));

root.Send(pid, new MessageId("1"));
root.Send(pid, new MessageId("1"));

await Task.Delay(3100);

root.Send(pid, new MessageId("1"));
root.Send(pid, new MessageId("1"));

await Task.Delay(6000);

await system.ShutdownAsync();


