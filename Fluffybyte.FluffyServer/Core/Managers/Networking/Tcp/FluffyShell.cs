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

    public int MeasureTerminalLatency()
    {
        // Send a cursor position query (ANSI escape sequence)
        // Format: ESC[6n
        // Terminal responds with: ESC[{row};{col}R
    
        var stream = _client.GetStream();
        var sw = System.Diagnostics.Stopwatch.StartNew();
    
        stream.Write(Encoding.ASCII.GetBytes("\e[6n"));
        stream.Flush();
    
        // Read response: ESC[##;##R
        StringBuilder response = new();
        while (true)
        {
            var b = stream.ReadByte();
            
            if (b == -1) 
                break;
            
            response.Append((char)b);
            
            if ((char)b == 'R') break; // End of response
        }
    
        sw.Stop();
        return Latency = (int)sw.ElapsedMilliseconds;
    }
}

/*
 *------------------------------------------------------------
 * (FluffyShell.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */