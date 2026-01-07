using System.Text;
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

        var fileRead = Archivist.ReadFile($@"E:\Temp\test.txt");

        if (fileRead == null || fileRead.Length == 0)
        {
            Scribe.Warn("File was not found or file contents were not properly translated.");
        }
        else
        {
            var fileContents = Encoding.UTF8.GetString(fileRead);
            Console.WriteLine(fileContents);
            
            Scribe.DisplayFileContents(fileContents);
        }

        Console.ReadLine();
        
        sysOp.RequestStopAsync().Wait();
    }
}