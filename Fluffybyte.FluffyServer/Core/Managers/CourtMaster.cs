/*
 * (SystemOperator.cs)
 *------------------------------------------------------------
 * Created - Wednesday, January 7, 2026@1:52:12 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Debugger;

namespace Fluffybyte.FluffyServer.Core.Managers;

public static class CourtMaster
{
    private const string Name = "CourtMaster";
    
    public static CancellationToken ShutdownToken { get; private set; }
    private static CancellationTokenSource _shutdownTokenSource = new CancellationTokenSource();

    private static bool _initialized;
    
    public static void Initialize(string hostAddress, int hostPort)
    {
        if (_initialized)
        {
            Scribe.Warn($"{Name}: System already initialized.  Skipping.");
            return;
        }
        
        try
        {
            
            _shutdownTokenSource = new CancellationTokenSource();
            ShutdownToken = _shutdownTokenSource.Token;

            _initialized = true;
        }
        catch (Exception ex)
        {
            Scribe.Error($"{Name}: Exception during initialization.", ex);
        }
    }

    public static void Shutdown()
    {
        if (ShutdownToken == CancellationToken.None)
        {
            Scribe.Warn($"Shutdown operation initiated, but no active shutdown token found. " +
                        $"Skipping shutdown process.");
            return;
        }

        try
        {
            _shutdownTokenSource.Cancel();
            _shutdownTokenSource.Dispose();

            ShutdownToken = CancellationToken.None;
            
            Scribe.Info("Shutdown operation initiated. Initiating server shutdown sequence...");

            _initialized = false;
            Scribe.Info($"The CourtMaster has shut operations down.");
        }
        catch (Exception ex)
        {
            Scribe.Error($"{Name}: Exception during shutdown.", ex);
        }
    }
    
    
}

/*
 *------------------------------------------------------------
 * (SystemOperator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */