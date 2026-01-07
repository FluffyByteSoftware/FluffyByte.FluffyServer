/*
 * (IFluffyOperator.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@10:21:36 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace Fluffybyte.FluffyServer.Core.Types;

/// <summary>
/// Attach this to any "system" level process that will be started by SystemOperator
/// </summary>
public interface IFluffyOperator
{


    FluffyOperationState State { get; }
    CancellationTokenRegistration ShutdownRegistration { get; }
    
    Task RequestStartAsync();
}

/*
 *------------------------------------------------------------
 * (IFluffyOperator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */