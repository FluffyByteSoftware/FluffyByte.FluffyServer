/*
 * (Archivist.cs)
 *------------------------------------------------------------
 * Created - Wednesday, January 7, 2026@12:26:34 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Debugger;

namespace Fluffybyte.FluffyServer.Core.Managers;

public static class Archivist
{
    private static readonly string Name = "Archivist";
    
    private const long FlushThresholdBytes = 1024 * 1024 * 35; // 35MB
    
    #region File Cacheing and Buffering

    private static readonly Dictionary<string, byte[]> FileCache = [];
    private static readonly Lock CacheLock = new Lock();

    private static readonly Dictionary<string, byte[]> WriteQueue = [];
    private static readonly Lock WriteLock = new Lock();
    
    /// <summary>
    /// Calculates the total size in bytes of all cached files.
    /// </summary>
    private static long GetCacheSize()
    {
        lock (CacheLock)
        {
            return FileCache.Values.Sum(data => data.LongLength);
        }
    }

    /// <summary>Reads the contents of a file at the specified path into a byte array. If the file has been
    /// previously read, the method attempts to retrieve the data from an in-memory cache to improve
    /// performance.</summary>
    /// <param name="filePath">The path of the file to be read. This should be a valid file path.</param>
    /// <returns>A byte array containing the contents of the file if the operation succeeds.
    /// Returns null if the file does not exist, the file path is invalid, or an error occurs during the read operation.</returns>
    public static byte[]? ReadFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return null;

        lock (CacheLock)
        {
            if (FileCache.TryGetValue(filePath, out var cached))
            {
                Scribe.Debug($"{Name}: Cache hit for {filePath}");
                return cached;
            }
        }

        try
        {
            if (!File.Exists(filePath))
            {
                Scribe.Warn($"{Name}: File not found on disk: {filePath}");
                return null;
            }

            var fileData = File.ReadAllBytes(filePath);

            lock (CacheLock)
            {
                FileCache[filePath] = fileData;
            }

            Scribe.Debug($"{Name}: Loaded from disk and cached: {filePath}");

            return fileData;
        }
        catch (Exception ex)
        {
            Scribe.Error($"{Name}: Exception during ReadFile: {ex}");
            return null;
        }
    }

    /// <summary>Writes the provided data to the specified file path and queues the write operation for further
    /// processing. The data is temporarily stored in an internal cache and added to the writing queue to ensure
    /// the operation completes in a consistent and controlled manner.</summary>
    /// <param name="filePath">The path of the file to which the data will be written. Must not be null
    /// or empty.</param>
    /// <param name="data">The byte array representing the data to write. Must not be null.</param>
    public static void WriteFile(string filePath, byte[]? data)
    {
        if (string.IsNullOrEmpty(filePath) || data == null)
        {
            return;
        }

        lock (CacheLock)
        {
            FileCache[filePath] = data;
        }

        lock (WriteLock)
        {
            // Queue this file for disk write on the next tick
            WriteQueue[filePath] = data;
        }

        Scribe.Debug($"{Name}: Queued write for {filePath}");
    }
    #endregion File Cacheing and Buffering
    
    #region Tick Operations

    /// <summary>Executes operations that need to occur periodically based on a tick count,
    /// handling tasks such as flushing the writing queue and logging errors.</summary>
    /// <param name="tickCount">The current tick count, representing the number of elapsed periods since the
    /// start of the daemon's operation.</param>
    /// <returns>A task representing the asynchronous operation, allowing the method to be awaited by
    /// callers.</returns>
    public static async Task Tick(long tickCount)
    {
        try
        {
            // Check if cache has exceeded maximum allowable size
            var cacheSize = GetCacheSize();

            if (cacheSize > FlushThresholdBytes)
            {
                Scribe.Warn($"{Name}: Cache size ({cacheSize} bytes) exceeds threshold " +
                         $"({FlushThresholdBytes} bytes). Force flushing.");
            }
            
            await FlushWriteQueue();
        }
        catch(Exception ex)
        {
            Scribe.Error($"{Name}: Exception during Tick({tickCount})", ex);
        }
    }

    /// <summary>
    /// Immediately flushes all pending writes to disk. Called during system shutdown.
    /// </summary>
    public static async Task ShutdownFlush()
    {
        try
        {
            Scribe.Info($"{Name}: ShutdownFlush() initiated.");
            await FlushWriteQueue();
            Scribe.Info($"{Name}: ShutdownFlush() completed successfully.");
        }
        catch (Exception ex)
        {
            Scribe.Error($"{Name}: Exception during ShutdownFlush()", ex);
        }
    }

    private static async Task FlushWriteQueue()
    {
        Dictionary<string, byte[]> pendingWrites;

        lock (WriteLock)
        {
            if (WriteQueue.Count == 0)
                return;
            
            pendingWrites = new Dictionary<string, byte[]>(WriteQueue);
            WriteQueue.Clear();
        }

        foreach (var (filePath, data) in pendingWrites)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrEmpty(directory) &&
                    !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllBytesAsync(filePath, data);

                Scribe.Debug($"{Name}: Flush to disk: {filePath}");
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown
                Scribe.Warn($"{Name}: FlushWriteQueue stopped due to shutdown.");
                Scribe.Error($"{Name}: FlushWriteQueue failed for {filePath}, with remaining " +
                             $"{data.Length} items.");
            }
            catch (Exception ex)
            {
                Scribe.Error($"{Name}: FlushWriteQueue failed for {filePath}; remaining: {data.Length}", ex);
            }
        }
    }
    #endregion Tick Operations
    
    
}

/*
 *------------------------------------------------------------
 * (Archivist.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */