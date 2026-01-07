/*
 * (OperationStatus.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@2:10:50 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace Fluffybyte.FluffyServer.Core.Types;

/// <summary>
/// Represents the possible states of an operation or a service.
/// </summary>
public enum FluffyOperationState
{
    /// <summary>
    /// Indicates that the operation or service is currently stopped.
    /// </summary>
    Stopped,

    /// <summary>
    /// Indicates that the operation or service is in the process of starting.
    /// </summary>
    Starting,

    /// <summary>
    /// Indicates that the operation or service is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// Indicates that the operation or service is in the process of stopping.
    /// </summary>
    Stopping,

    /// <summary>
    /// Indicates that the operation or service encountered an error or failure.
    /// </summary>
    Error
}

/*
 *------------------------------------------------------------
 * (OperationStatus.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */