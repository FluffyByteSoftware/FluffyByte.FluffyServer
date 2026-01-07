using FluffyByte.Debugger;
using Fluffybyte.FluffyServer.Core.Managers;

namespace FluffyByte.FluffyServer.Core.Bootstrap;

public static class Program
{
    public static void Main(string[] args)
    {
        var sysOp = new SystemOperator("10.0.0.83", 9997);

        sysOp.RequestStartAsync().Wait();

        Scribe.Info("Should have started successfully...");

        Scribe.Info($"{sysOp.State} is the current state of sys op.");

        Console.ReadLine();
        
        sysOp.RequestStopAsync().Wait();
    }
}