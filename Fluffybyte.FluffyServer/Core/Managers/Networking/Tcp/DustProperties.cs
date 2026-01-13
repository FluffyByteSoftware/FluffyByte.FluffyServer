/*
 * (DustProperties.cs)
 *------------------------------------------------------------
 * Created - Monday, January 12, 2026@6:20:15 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace Fluffybyte.FluffyServer.Core.Managers.Networking.Tcp;

public sealed class DustProperties
{
    public Guid Guid { get; private set; } = Guid.NewGuid();
    
    public string Name { get; private set; }
    private Dust _parent;

    private string _address;
    
    public DustProperties(Dust dust)
    {
        _parent = dust;
        _address = dust._client.Client.RemoteEndPoint.ToString();
    }
}

/*
 *------------------------------------------------------------
 * (DustProperties.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */