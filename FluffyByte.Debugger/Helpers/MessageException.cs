/*
 * (MessageException.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@10:20:57 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Debugger.Helpers;

/// <summary>
/// Represents an exception object with enhanced details for diagnostic purposes, including:
/// the exception type, message, stack trace, and inner exception chain (up to 10 levels).
/// </summary>
/// <remarks>
/// This class is primarily used for capturing and formatting exception details in a structured manner.
/// It encapsulates information from the provided exception, making it suitable for logging or debugging scenarios.
/// Nested inner exceptions are also represented in a recursive hierarchy within the object.
/// </remarks>
public class MessageException
{
    /// <summary>
    /// Gets the type identifier for the current instance.
    /// </summary>
    public string Type { get; }
    /// <summary>
    /// Gets the Message content associated with this instance.
    /// </summary>
    public string Message { get; }
    /// <summary>
    /// Gets a string representation of the immediate frames on the call stack at the time the exception was thrown.
    /// </summary>
    /// <remarks>The stack trace provides information that can be useful for debugging, such as the sequence
    /// of method calls that led to the exception. The value may be null if no stack trace is available.</remarks>
    public string? StackTrace { get; }
    /// <summary>
    /// Gets the exception information for the inner exception, if one exists.
    /// </summary>
    public MessageException? Inner { get; }

    /// <summary>
    /// Initializes a new instance of the ExceptionInfo class using the specified exception.
    /// </summary>
    /// <remarks>This constructor captures the type, Message, and stack trace of the provided exception. If
    /// the exception has inner exceptions, up to 10 levels of inner exception information are recursively included.
    /// This helps in representing complex exception chains for diagnostic or logging purposes.</remarks>
    /// <param name="ex">The exception to extract information from. Cannot be null.</param>
    public MessageException(Exception ex)
    {
        Type         =  ex.GetType().FullName ?? "UnknownExceptionType";
        Message      =  ex.Message;
        StackTrace   =  ex.StackTrace;

        int depth = 0;

        while(ex.InnerException is not null && depth < 10)
        {
            Inner = new MessageException(ex.InnerException);
            ex = ex.InnerException;

            depth++;

            if (depth == 9 && Inner is not null)
            {
                Message += "\nThere were additional inner exceptions that were omitted.\n";
            }
        }
        
        
    }
}

/*
 *------------------------------------------------------------
 * (MessageException.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */