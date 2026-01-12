/*
 * (Program.cs)
 *------------------------------------------------------------
 * Created - Thursday, January 8, 2026@12:22:27 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Debugger;
using Fluffybyte.FluffyServer.Core.Managers;

namespace Fluffybyte.FluffyServer.Core.Bootstrap;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var hostAddress = "10.0.0.84";
        var hostPort = 9997;
        
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-ip":
                {
                    var possibleIp = args[i + 1];
                
                    if(possibleIp.Contains('.'))
                        hostAddress = args[i + 1];
                    else 
                        Scribe.Warn($"Invalid IP Address: {possibleIp}");
                    break;
                }
                case "-port" when int.TryParse(args[i + 1], out var parsedPort):
                    hostPort = parsedPort;
                    break;
                case "-port":
                    Scribe.Warn($"Invalid Port: {args[i + 1]}");
                    break;
            }
        }
        Scribe.Info($"Host Address: {hostAddress} -port {hostPort}");
        
        CourtMaster.Initialize(hostAddress, hostPort);
        Archivist.RegisterShutdown(CourtMaster.ShutdownToken);

        Console.ReadLine();
        
        CourtMaster.Shutdown();

        Scribe.Info("Server shutdown complete.");

        await Task.CompletedTask;
    }
}

/*
 *------------------------------------------------------------
 * (Program.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */