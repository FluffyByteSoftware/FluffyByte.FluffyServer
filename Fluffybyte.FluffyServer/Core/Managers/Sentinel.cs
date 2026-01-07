/*
 * (Sentinel.cs)
 *------------------------------------------------------------
 * Created - Wednesday, January 7, 2026@12:16:15 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Debugger;
using Fluffybyte.FluffyServer.Core.Types;

namespace Fluffybyte.FluffyServer.Core.Managers;

public sealed class Sentinel : IFluffyOperator
{
    public FluffyOperationState State { get; private set; } = FluffyOperationState.Stopped;

    public CancellationTokenRegistration ShutdownRegistration { get; private set; }
    private string _hostAddress;
    private int _hostPort;

    private SystemOperator Parent;
    
    public Sentinel(string hostAddress, int hostPort, SystemOperator parent)
    {
        _hostAddress = hostAddress;
        _hostPort = hostPort;
        Parent = parent;
        
        ShutdownRegistration =
            CancellationTokenSource.CreateLinkedTokenSource(Parent.ShutdownToken)
                .Token.Register(
                    () => ShutdownInitiated());
    }
    
    public async Task RequestStartAsync()
    {
        if (State is not FluffyOperationState.Stopped)
        {
            
        }
        
        try
        {
            State = FluffyOperationState.Starting;

            Scribe.Debug($"Sentinel is now requesting NetManager to load...");
        }
        catch (Exception ex)
        {
            Scribe.Critical(ex);
        }

        await Task.CompletedTask;
    }

    private void ShutdownInitiated()
    {
        
    }
}

/*
 *------------------------------------------------------------
 * (Sentinel.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */