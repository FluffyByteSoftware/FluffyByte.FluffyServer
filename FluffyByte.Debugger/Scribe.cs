/*
 * (Scribe.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@10:25:38 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */
using System.Runtime.CompilerServices;

using FluffyByte.Debugger.Helpers;

namespace FluffyByte.Debugger;

public static class Scribe
{
    /// <summary>
    /// Marks whether the Logger should display Debug messages or not.
    /// Useful for debugging.
    /// </summary>
    public static bool DebugModeEnabled { get; set; } = true;
    
    
    #region Public Methods

    /// <summary>
    /// Displays the specified file contents to the console output.
    /// </summary>
    /// <param name="fileContents">The contents of the file to display. If null or empty, no content will be shown
    /// after the header.</param>
    public static void DisplayFileContents(string fileContents)
    {
        Console.WriteLine("File Contents: ");
        Console.WriteLine($"{fileContents}");
    }
    
    /// <summary>
    /// Logs an informational message with contextual source information, such as the calling line number, member name,
    /// and file path.
    /// </summary>
    /// <remarks>Use this method to record non-error, informational events that may be useful for diagnostics
    /// or auditing. The contextual parameters are automatically populated and help identify the source of the log
    /// entry.</remarks>
    /// <param name="message">The message to log. This should describe the informational event or state to
    /// record.</param>
    /// <param name="line">The line number in the source file where the method is called. This value is automatically
    /// supplied by the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the log entry originates.
    /// This value is automatically supplied by the compiler.</param>
    /// <param name="file">The full path of the source file where the method is called. This value is automatically
    /// supplied by the compiler.</param>
    public static void Info(
        string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
        => CreateEnvelope(MessageSeverity.Info, message, null, line, member, file);

    /// <summary>
    /// Writes a debug-level message to the application's debug log, including caller information such as line number,
    /// member name, and file path.
    /// </summary>
    /// <remarks>This method only writes the message if debug mode is enabled. Caller information is included
    /// to assist with tracing and diagnostics. The caller info parameters are typically not specified explicitly; they
    /// are populated by the compiler using the Caller Info attributes.</remarks>
    /// <param name="message">The message to log at the debug level. This should describe the event or state to be
    /// recorded.</param>
    /// <param name="line">The line number in the source code where the method was called. This value is automatically
    /// supplied by the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the log method was called.
    /// This value is automatically supplied by the compiler.</param>
    /// <param name="file">The full path of the source file containing the caller. This value is automatically
    /// supplied by the compiler.</param>
    public static void Debug(
        string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
    {
        if(!DebugModeEnabled)
            return;

        CreateEnvelope(MessageSeverity.Debug, message, null, line, member, file);
    }

    /// <summary>
    /// Logs a debug-level message for the specified exception, including caller information such as line number, member
    /// name, and file path.
    /// </summary>
    /// <remarks>This method only logs the exception if debug mode is enabled. Caller information is captured
    /// using compiler-provided attributes, which can assist in tracing the origin of the logged exception.</remarks>
    /// <param name="ex">The exception to log. The exception's message will be included in the debug output.</param>
    /// <param name="line">The line number in the source file where the method was called. This value is
    /// automatically provided by the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the method was called.
    /// This value is automatically provided by the compiler.</param>
    /// <param name="file">The full path of the source file where the method was called. This value is automatically
    /// provided by the compiler.</param>
    public static void Debug(
        Exception ex,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
    {
        if (!DebugModeEnabled)
            return;

        CreateEnvelope(MessageSeverity.Debug, ex.Message, ex, line, member, file);
    }

    /// <summary>
    /// Writes a debug-level log entry with the specified message and exception details, including caller information
    /// such as line number, member name, and file path.
    /// </summary>
    /// <remarks>This method only writes the log entry if debug mode is enabled. Caller information is
    /// automatically populated using compiler services attributes, which can assist in tracing the origin of log
    /// messages.</remarks>
    /// <param name="message">The message to include in the debug log entry. This should describe the event or
    /// condition being logged.</param>
    /// <param name="ex">The exception to log with the message. Can be null if no exception is associated with the
    /// log entry.</param>
    /// <param name="line">The line number in the source code where the log entry is generated. This is typically
    /// supplied automatically by
    /// the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the log entry is generated.
    /// This is typically
    /// supplied automatically by the compiler.</param>
    /// <param name="file">The full path of the source file where the log entry is generated. This is typically
    /// supplied automatically by
    /// the compiler.</param>
    public static void Debug(
        string message,
        Exception ex,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
    {
        if(!DebugModeEnabled)
            return;
        CreateEnvelope(MessageSeverity.Debug, message, ex, line, member, file);
    }

    /// <summary>
    /// Logs a warning message with contextual information about the source location.
    /// </summary>
    /// <remarks>Use this method to record non-critical issues that may require attention but do not prevent
    /// normal operation. Caller information parameters are automatically populated; you typically do not need to
    /// specify them manually.</remarks>
    /// <param name="message">The warning message to log. This should describe the condition or issue that triggered
    /// the warning.</param>
    /// <param name="line">The line number in the source file where the warning is logged. This value is automatically
    /// supplied by the
    /// compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the warning is logged. This
    /// value is automatically
    /// supplied by the compiler.</param>
    /// <param name="file">The full path of the source file where the warning is logged. This value is automatically
    /// supplied by the
    /// compiler.</param>
    public static void Warn(
        string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
        => CreateEnvelope(MessageSeverity.Warn, message, null, line, member, file);

    /// <summary>
    /// Logs a warning message based on the specified exception, including caller information for diagnostic purposes.
    /// </summary>
    /// <remarks>Caller information parameters are populated automatically; they do not need to be specified
    /// explicitly in typical usage. This method is intended for logging exceptions as warnings, aiding in debugging
    /// and error tracking.</remarks>
    /// <param name="ex">The exception to log as a warning. The exception's message will be included in the log entry.
    /// Cannot be null.</param>
    /// <param name="line">The line number in the source file where the method was called. This value is automatically
    /// supplied by the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the method was called.
    /// This value is automatically supplied by the compiler.</param>
    /// <param name="file">The full path of the source file where the method was called. This value is automatically
    /// supplied by the compiler.</param>
    public static void Warn(
        Exception ex,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
        => CreateEnvelope(MessageSeverity.Warn, ex.Message, ex, line, member, file);

    /// <summary>
    /// Logs a warning message along with an optional exception and caller information for diagnostic purposes.
    /// </summary>
    /// <remarks>Caller information parameters (<paramref name="line"/>, <paramref name="member"/>, and
    /// <paramref name="file"/>) are automatically populated by the compiler and do not need to be specified manually
    /// in most cases.</remarks>
    /// <param name="message">The warning message to log. This should describe the issue or condition that
    /// triggered the warning.</param>
    /// <param name="ex">The exception associated with the warning, if any. Specify <see langword="null"/>
    /// if no exception is relevant.</param>
    /// <param name="line">The line number in the source code where the warning was generated. This is
    /// typically supplied automatically by the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the warning was logged.
    /// This is typically supplied automatically by the compiler.</param>
    /// <param name="file">The full path of the source file where the warning was generated. This is typically
    /// supplied automatically by the compiler.</param>
    public static void Warn(
        string message,
        Exception ex,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
        => CreateEnvelope(MessageSeverity.Warn, message, ex, line, member, file);

    /// <summary>
    /// Logs an error message with contextual information about the source location.
    /// </summary>
    /// <remarks>Use this method to record error-level events along with source code context for easier
    /// debugging. Caller information parameters are automatically populated; you typically do not need to specify them
    /// manually.</remarks>
    /// <param name="message">The error message to log. This value cannot be null.</param>
    /// <param name="line">The line number in the source file where the error occurred. This value is automatically
    /// supplied by the compiler.</param>
    /// <param name="member">The name of the member (such as method or property) where the error occurred.
    /// This value is automatically supplied by the compiler.</param>
    /// <param name="file">The full path of the source file where the error occurred. This value is automatically
    /// supplied by the compiler.</param>
    public static void Error(
        string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
        => CreateEnvelope(MessageSeverity.Error, message, null, line, member, file);

    /// <summary>
    /// Logs an error event with the specified exception and caller information.
    /// </summary>
    /// <remarks>Caller information parameters are automatically populated by the compiler and typically do
    /// not need to be specified manually. Use this method to record error details for diagnostic or debugging
    /// purposes.</remarks>
    /// <param name="ex">The exception to log. Cannot be null.</param>
    /// <param name="line">The line number in the source file where the error occurred. This value is
    /// automatically supplied by the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) where the error occurred.
    /// This value is automatically supplied by the compiler.</param>
    /// <param name="file">The full path of the source file where the error occurred. This value is automatically
    /// supplied by the compiler.</param>
    public static void Error(
        Exception ex,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
        => CreateEnvelope(MessageSeverity.Error, ex.Message, ex, line, member, file);

    /// <summary>
    /// Logs an error message along with exception details and caller information for diagnostic purposes.
    /// </summary>
    /// <remarks>Caller information parameters are automatically populated by the compiler and typically do
    /// not need to be specified manually. Use this method to record errors with detailed context for
    /// troubleshooting.</remarks>
    /// <param name="message">The error message to log. Provides context about the error condition.</param>
    /// <param name="ex">The exception associated with the error. Contains details about the error that
    /// occurred.</param>
    /// <param name="line">The line number in the source code where the error was logged. This value is automatically
    /// supplied by the compiler.</param>
    /// <param name="member">The name of the member (method, property, etc.) from which the error was logged.
    /// This value is automatically supplied by the compiler.</param>
    /// <param name="file">The full path of the source file where the error was logged. This value is automatically
    /// supplied by the compiler.</param>
    public static void Error(
        string message,
        Exception ex,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "")
        => CreateEnvelope(MessageSeverity.Error, message, ex, line, member, file);
    #endregion

    #region Private Methods
    /// <summary>
    /// Creates a log envelope containing the specified debug information and writes it to the console with a color
    /// corresponding to the severity level.
    /// </summary>
    /// <remarks>The console foreground color is temporarily changed based on the severity and restored after
    /// writing the log entry. This method is intended for internal use in logging debug information.</remarks>
    /// <param name="severity">The severity level of the debug message, which determines the console text
    /// color.</param>
    /// <param name="message">The message to include in the log envelope.</param>
    /// <param name="ex">An optional exception to include in the log envelope. If not null, exception details are
    /// captured.</param>
    /// <param name="line">The line number in the source file where the log was generated.</param>
    /// <param name="member">The name of the member (method or property) from which the log was generated.</param>
    /// <param name="file">The path of the source file where the log was generated.</param>
    private static void CreateEnvelope(
        MessageSeverity severity,
        string message,
        Exception? ex,
        int line,
        string member,
        string file)
    {
        var envelope = new MessageEnvelope(
            severity,
            message,
            ex != null ? new MessageException(ex) : null,
            file,
            line,
            member
        );

        ConsoleColor fg = Console.ForegroundColor;

        Console.ForegroundColor = severity switch
        {
            MessageSeverity.Debug => ConsoleColor.Green,
            MessageSeverity.Info => ConsoleColor.White,
            MessageSeverity.Warn => ConsoleColor.Yellow,
            MessageSeverity.Error => ConsoleColor.Red,
            _ => ConsoleColor.Blue,
        };

        Console.WriteLine(envelope.ToString());

        Console.ForegroundColor = fg;
    }
    #endregion
}

/*
 *------------------------------------------------------------
 * (Scribe.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */