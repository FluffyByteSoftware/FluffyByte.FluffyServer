/*
 * (FluffyShell.cs)
 *------------------------------------------------------------
 * Created - Sunday, January 11, 2026@8:24:31 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net.Sockets;
using System.Text;
using FluffyByte.Debugger;

namespace Fluffybyte.FluffyServer.Core.Managers.Networking.Tcp;

/// <summary>
/// Wraps around the TcpClient for initial server welcome, and authentication.
/// </summary>
public class FluffyShell
{
    private readonly TcpClient _client;
    private static int _id;
    private readonly string _name;

    private readonly string _address;
    public int Latency { get; private set; }

    public FluffyShell(TcpClient client)
    {
        _client = client;

        _id++;
        
        _address = client.Client.RemoteEndPoint?.ToString() ?? "0.0.0.1";
        
        Latency = 0;
        
        _name = $"FluffyShell_{_id}";
    }
    
    public override string ToString()
    {
        return $"{_name}@({_address})";
    }

    public async ValueTask SendMessage(byte[] message)
    {
        var messageString = Encoding.UTF8.GetString(message);
        
        try
        {
            await _client.GetStream().WriteAsync(message);
            Scribe.Debug($"Sending Message: {messageString}");
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
            Scribe.Warn($"{_name}: SendMessage cancelled due to shutdown.");
            
        }
        catch (Exception ex)
        {
            Scribe.Error($"{_name} experienced an exception during SendMessage.", ex);
        }
    }

    public async ValueTask<byte[]?> ReceiveMessage()
    {
        try
        {
            var stream = _client.GetStream();
            var buffer = new byte[1024];
            var bytesRead = await stream.ReadAsync(buffer, CourtMaster.ShutdownToken);

            return bytesRead == 0 ? null : buffer.Take(bytesRead).ToArray();
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
            return null;
        }
        catch (Exception ex)
        {
            Scribe.Error($"Error in ReceiveMessage for {_name}", ex);
            return null;
        }
    }

    public async Task<int> PingAsync()
    {
        try
        {
            var stream = _client.GetStream();

            // Send ping timestamp
            var sendTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var pingData = BitConverter.GetBytes(sendTime);

            await stream.WriteAsync(pingData, CourtMaster.ShutdownToken);
            await stream.FlushAsync(CourtMaster.ShutdownToken);

            // Wait for echo response
            var buffer = new byte[8];
            var bytesRead = await stream.ReadAsync(buffer, CourtMaster.ShutdownToken);

            if (bytesRead != 8)
                return -1;

            var receiveTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var echoedTime = BitConverter.ToInt64(buffer);

            // Verify it's our timestamp
            if (echoedTime != sendTime)
                return -1;

            Latency = (int)(receiveTime - sendTime);
            return Latency;
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
            return -1;
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
            return -1;
        }
    }
}

/*
 *------------------------------------------------------------
 * (FluffyShell.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */