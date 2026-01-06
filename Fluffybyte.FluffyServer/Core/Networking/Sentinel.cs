/*
 * (Sentinel.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@1:32:42 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */
using System.Net;
using System.Net.Sockets;

namespace Fluffybyte.FluffyServer.Core.Networking;

public sealed class Sentinel
{
    private Socket _tcpSocket;
    private Socket _udpSocket;

    private int _hostPort;
    private string _hostAddress;
    
    public Sentinel(string hostAddress, int hostPort)
    {
        _hostPort = hostPort;
        _hostAddress = hostAddress;

        _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
    }

    public async Task RequestStart()
    {
        
    }
}

/*
 *------------------------------------------------------------
 * (Sentinel.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */