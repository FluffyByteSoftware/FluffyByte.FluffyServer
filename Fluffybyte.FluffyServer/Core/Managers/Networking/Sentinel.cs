/*
 * (Sentinel.cs)
 *------------------------------------------------------------
 * Created - Sunday, January 11, 2026@8:24:16 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net;
using System.Net.Sockets;
using System.Text;
using FluffyByte.Debugger;
using Fluffybyte.FluffyServer.Core.Managers.Networking.Tcp;
using Fluffybyte.FluffyServer.Core.Types;

namespace Fluffybyte.FluffyServer.Core.Managers.Networking;

public static class Sentinel
{
    private const string Name = "Sentinel";
    private static TcpListener? _listener;
    private static bool _active;
    private static readonly Lock Lock = new();
    private static FluffyOperationState _state = FluffyOperationState.Stopped;
    
    public static void Watch(string address, int port)
    {
        if (_state != FluffyOperationState.Stopped)
        {
            Scribe.Critical($"{Name}: Already running or errored.  Please check the log.");
            return;
        }
        
        lock (Lock)
        {
            if (_active) return;
            
            _active = true;
        }

        try
        {
            var ipAddress = IPAddress.Parse(address);
            
            _listener = new TcpListener(ipAddress, port);
            _listener.Start();

            Scribe.Info($"{Name}: Vigil started on port {port}.  The gates are open.");

            _state = FluffyOperationState.Started;
            
            // Begin the asynchronous acceptance loop
            _ = Task.Run(AcceptLoop, CourtMaster.ShutdownToken);
        }
        catch (OperationCanceledException)
        {
            Scribe.Warn($"Sentinel watch interrupted by shutdown.");
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }

    private static async Task AcceptLoop()
    {
        while (!CourtMaster.ShutdownToken.IsCancellationRequested)
        {
            try
            {
                // Accept an incoming client
                TcpClient client = await _listener!.AcceptTcpClientAsync(CourtMaster.ShutdownToken);

                Scribe.Debug($"A new client has arrived.");

                var fs = new Dust(client);

                const string helloWorld = "Hello World!";
                
                await fs.SendMessage(Encoding.UTF8.GetBytes(helloWorld));

                var response = await fs.ReceiveMessage();
                
                var responseString = Encoding.UTF8.GetString(response ?? Array.Empty<byte>());
                Scribe.Debug($"Received Message: {responseString}");
                
                // Hand off to the warden
                //_ = Warden.Examine(client);
            }
            catch (OperationCanceledException)
            {
                // Expected during a shutdown
                Scribe.Warn($"Sentinel accept loop interrupted by shutdown.");
            }
            catch (Exception ex)
            {
                if(!CourtMaster.ShutdownToken.IsCancellationRequested)
                    Scribe.Error(ex);
            }
        }

        Stop();
    }

    private static void Stop()
    {
        if (_state is not FluffyOperationState.Started)
        {
            Scribe.Warn($"{Name}: is not running but was asked to stop.");
            return;
        }

        try
        {
            lock (Lock)
            {
                _active = false;
                _listener?.Stop();

                Scribe.Info($"{Name}: The Sentinel has closed the gates.");

                _state = FluffyOperationState.Stopped;
            }
        }
        catch (Exception ex)
        {
            if (!CourtMaster.ShutdownToken.IsCancellationRequested)
                Scribe.Error(ex);
        }
    }
}

/*
 *------------------------------------------------------------
 * (Sentinel.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */