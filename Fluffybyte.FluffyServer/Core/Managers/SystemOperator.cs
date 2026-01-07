/*
 * (SystemOperator.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@1:22:58 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Debugger;
using Fluffybyte.FluffyServer.Core.Types;

namespace Fluffybyte.FluffyServer.Core.Managers;

public sealed class SystemOperator
{
    public FluffyOperationState State { get; private set; } = FluffyOperationState.Stopped;

    private readonly Sentinel _sentinel;

    private CancellationTokenSource _cts;
    public CancellationToken ShutdownToken { get; private set; }

    public SystemOperator(string hostAddress, int hostPort)
    {
        _cts = new CancellationTokenSource();
        ShutdownToken = _cts.Token;
        
        _sentinel = new Sentinel(hostAddress, hostPort, this);
    }
    
    public async Task RequestStartAsync()
    {
        if (State != FluffyOperationState.Stopped)
        {
            Scribe.Critical($"System Operator was in state: {State} and could not be started.");
            return;
        }
        
        try
        {
            _cts = new();
            ShutdownToken = _cts.Token;

            Scribe.Debug($"System Operator is now requesting startup of services...");

            await _sentinel.RequestStartAsync();

            Scribe.Debug($"Sentinel state is now: {_sentinel.State}");
            
            State = FluffyOperationState.Running;

            Scribe.Debug($"System Operator is now running.");
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }

    public async Task RequestStopAsync()
    {
        if (State != FluffyOperationState.Running)
        {
            Scribe.Critical($"System Operator was in state: {State} and could not be stopped.");
            return;
        }

        try
        {
            State = FluffyOperationState.Stopping;
            
            Scribe.Debug($"System Operator is now requesting a shutdown of services.");

            Scribe.Info($"Cancelling shutdown token!");
            
            await _cts.CancelAsync();

            State = FluffyOperationState.Stopped;
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }
}

/*
 *------------------------------------------------------------
 * (SystemOperator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */