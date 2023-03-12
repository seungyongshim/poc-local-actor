using Proto;

var system = new ActorSystem();

var root = system.Root;

var props = Props.FromProducer(() =>
                new ProxyActor(Props.FromProducer(() =>
                    new ProxyActor(Props.FromProducer(() =>
                        new ConsoleActor())))));

var pid = root.SpawnNamed(props, nameof(ProxyActor));

root.Send(pid, new MessageId("1"));
root.Send(pid, new MessageId("1"));

await Task.Delay(3100);

root.Send(pid, new MessageId("1"));
root.Send(pid, new MessageId("1"));

await Task.Delay(5000);

await system.ShutdownAsync();


