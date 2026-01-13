/*
 * (Dust.cs)
 *------------------------------------------------------------
 * Created - Monday, January 12, 2026@6:17:44 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net.Sockets;

namespace Fluffybyte.FluffyServer.Core.Managers.Networking.Tcp;

/// <summary>
/// Represents a raw TCP connection from a client. Dust is the Initial, unauthenticated state
/// of a connection-just particles in the network.  It handles low-level I/O and tracks the progression from
/// initial connection through authentication and character selection.
/// </summary>
public class Dust : IDisposable
{
    internal readonly TcpClient _client;
    private readonly NetworkStream _stream;

    private bool _disposed;
    
    internal DustPattern _pattern;
    internal DustProperties _properties;

    public Dust(TcpClient client)
    {
        _client = client;
        _stream = _client.GetStream();

        _pattern = DustPattern.New;
        _properties = new DustProperties(this);
    }
    
    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        
        _client.Close();
        _stream.Close();

        _client.Dispose();
        _stream.Dispose();
        
        GC.SuppressFinalize(this);
    }
}

/*
 *------------------------------------------------------------
 * (Dust.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */