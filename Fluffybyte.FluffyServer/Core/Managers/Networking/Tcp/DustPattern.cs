/*
 * (DustPattern.cs)
 *------------------------------------------------------------
 * Created - Monday, January 12, 2026@6:06:31 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace Fluffybyte.FluffyServer.Core.Managers.Networking.Tcp;

/// <summary>
/// The DustPattern is the current state of a "Dust" (TcpClient wrapper).
/// </summary>
public enum DustPattern
{
    /// <summary>
    /// Initial state. Connection accepted but not yet authenticated. Raw, unnamed potential.
    /// </summary>
    New,

    /// <summary>
    /// Client is in the process of authenticating. Credentials have been received and are being validated.
    /// </summary>
    Authenticating,

    /// <summary>
    /// Authentication successful. Client is selecting which character to play from their account.
    /// </summary>
    CharacterSelect,

    /// <summary>
    /// Connection closed or authentication failed. Final state before cleanup.
    /// </summary>
    Disconnect
}

/*
 *------------------------------------------------------------
 * (DustPattern.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */