/*
 * (MessageSeverity.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@10:20:14 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Debugger.Helpers;

/// <summary>
/// Represents the severity level of a message.
/// </summary>
public enum MessageSeverity
{
    /// <summary>Debug mode message, typically used for development and troubleshooting.</summary>
    Debug,
    /// <summary>Info mode message, used for general informational purposes.</summary>
    Info,
    /// <summary>Warn mode message, used for warning conditions that are not errors but may require
    /// attention.</summary>
    Warn,
    /// <summary>Error mode message, used for error conditions that indicate a failure in a specific
    /// operation.</summary>
    Error,

    /// <summary>
    /// Critical mode message, indicating severe or fatal issues that require immediate attention.
    /// WARNING: CAUSES AN IMMEDIATE SHUTDOWN OF THE APPLICATION.
    /// </summary>
    Critical
}

/*
 *------------------------------------------------------------
 * (MessageSeverity.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */