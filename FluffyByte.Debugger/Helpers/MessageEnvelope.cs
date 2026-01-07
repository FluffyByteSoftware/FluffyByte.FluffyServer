/*
 * (MessageEnvelope.cs)
 *------------------------------------------------------------
 * Created - Tuesday, January 6, 2026@10:18:53 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Text;

namespace FluffyByte.Debugger.Helpers;

public class MessageEnvelope
{
    #region Variables
    /// <summary>
    /// Gets the date and time when the event occurred or the data was recorded.
    /// </summary>
    public DateTime Timestamp { get; init; }
    /// <summary>
    /// Gets the severity level associated with the debug Message.
    /// </summary>
    public MessageSeverity Severity { get; init; }
    /// <summary>
    /// Gets the Message content associated with this instance.
    /// </summary>
    public string Message { get; init; }
    /// <summary>
    /// Gets information about the exception that occurred during the operation, if any.
    /// </summary>
    public MessageException? Exception { get; init; }
    /// <summary>
    /// Gets the path of the source file associated with this instance.
    /// </summary>
    public string SourceFile { get; init; }
    /// <summary>
    /// Gets the line number in the source file associated with this instance.
    /// </summary>
    public int SourceLine { get; init; }
    /// <summary>
    /// Gets the identifier of the user or process that initiated the operation.
    /// </summary>
    public string Caller { get; init; }

    // Fixed-width box drawing infrastructure

    private const char Tl = '╔';
    private const char Tr = '╗';

    private const char Bl = '╚';
    private const char Br = '╝';

    private const char Mr = '╢';
    private const char Ml = '╟';

    private const char Hl = '═';
    private const char Vl = '║';

    private const int BoxWidth = 80;
    private const int InnerWidth = BoxWidth - 4; // | + space + content + space + |
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the LogEnvelope class with the specified severity, message, and optional exception
    /// and source information.
    /// </summary>
    /// <param name="severity">The severity level of the log entry, indicating its importance or type.</param>
    /// <param name="message">The message to be logged. This should describe the event or condition being recorded.</param>
    /// <param name="exception">Optional exception information associated with the log entry. Specify null if no exception is related.</param>
    /// <param name="sourceFile">The source file path where the log entry originated. If not specified, defaults to an empty string.</param>
    /// <param name="sourceLine">The line number in the source file where the log entry was generated. If not specified, defaults to 0.</param>
    /// <param name="caller">The name of the method or member that generated the log entry. If not specified, defaults to an empty string.</param>
    
    public MessageEnvelope(
        MessageSeverity severity,
        string message,
        MessageException? exception = null,
        string sourceFile = "",
        int sourceLine = 0,
        string caller = ""
        )
    {
        Timestamp = DateTime.UtcNow;
        Severity = severity;
        Message = message;
        Exception = exception;
        SourceFile = sourceFile;
        SourceLine = sourceLine;
        Caller = caller;
    }
    #endregion

    #region Box Drawing Helpers
    /// <summary>
    /// Creates a horizontal box line using the specified left and right border characters.
    /// </summary>
    /// <param name="left">The character to use for the left border of the box line.</param>
    /// <param name="right">The character to use for the right border of the box line.</param>
    /// <returns>A string representing a horizontal box line with the given left and right border characters and a horizontal
    /// line in between.</returns>
    private static string BoxLine(char left, char right)
        => $"{left}{new string(Hl, BoxWidth - 2)}{right}";

    /// <summary>
    /// Formats the specified content as a boxed line, aligning it within a fixed-width area and surrounding it with
    /// vertical line characters.
    /// </summary>
    /// <param name="content">The text to be formatted and placed inside the box. If null, the resulting line will contain only padding
    /// between the vertical lines.</param>
    /// <returns>A string representing the boxed line with the content aligned and enclosed by vertical line characters.</returns>
    private static string BoxContent(string content)
        => $"{Vl} {content,-InnerWidth} {Vl}";

    /// <summary>
    /// Wraps a labeled field value across multiple lines, maintaining proper indentation.
    /// </summary>
    /// <param name="label">The field label (will be padded to labelWidth).</param>
    /// <param name="value">The value to wrap.</param>
    /// <param name="labelWidth">Width to pad the label to (default 9 for standard fields).</param>
    /// <param name="baseIndent">Additional indentation prefix for nested content.</param>
    private static IEnumerable<string> WrapField(
        string label,
        string value,
        int labelWidth = 9,
        string baseIndent = "")
    {
        string prefix = $"{baseIndent}{label.PadRight(labelWidth)}: ";
        string continuation = new(' ', prefix.Length);
        int available = InnerWidth - prefix.Length;

        if (available < 10)
            available = 10;

        // Handle empty or whitespace values
        if (string.IsNullOrEmpty(value))
        {
            yield return BoxContent(prefix);
            yield break;
        }

        // Split by existing newlines first, then wrap each segment
        var segments = value.Split('\n');

        bool firstSegment = true;
        foreach (var segment in segments)
        {
            string trimmed = segment.TrimEnd('\r');

            if (trimmed.Length == 0)
            {
                yield return BoxContent(firstSegment ? prefix : continuation);
                firstSegment = false;
                continue;
            }

            for (int i = 0; i < trimmed.Length; i += available)
            {
                string chunk = trimmed.Substring(i, Math.Min(available, trimmed.Length - i));

                if (firstSegment && i == 0)
                    yield return BoxContent(prefix + chunk);
                else
                    yield return BoxContent(continuation + chunk);
            }

            firstSegment = false;
        }
    }

    /// <summary>
    /// Wraps raw text (like stack traces) with a fixed indentation prefix.
    /// </summary>
    private static IEnumerable<string> WrapText(string text, string indent = "")
    {
        int available = InnerWidth - indent.Length;

        if (available < 10)
            available = 10;

        var lines = text.Split('\n');

        foreach (var line in lines)
        {
            string trimmed = line.TrimEnd('\r');

            if (trimmed.Length == 0)
            {
                yield return BoxContent(indent);
                continue;
            }

            for (int i = 0; i < trimmed.Length; i += available)
            {
                string chunk = trimmed.Substring(i, Math.Min(available, trimmed.Length - i));
                yield return BoxContent(indent + chunk);
            }
        }
    }

    /// <summary>
    /// Appends a formatted field to the specified <see cref="StringBuilder"/>, aligning the label and value with
    /// optional width and indentation.
    /// </summary>
    /// <remarks>Each field is wrapped and formatted so that the label and value are aligned according to the
    /// specified width and indentation. This method is useful for generating readable, column-aligned text
    /// output.</remarks>
    /// <param name="sb">The <see cref="StringBuilder"/> to which the formatted field will be appended.</param>
    /// <param name="label">The label text to display for the field. Cannot be null.</param>
    /// <param name="value">The value text to display for the field. Cannot be null.</param>
    /// <param name="labelWidth">The minimum width, in characters, to use for the label column. Must be non-negative. The default is 9.</param>
    /// <param name="indent">An optional string to prepend to each line for indentation. The default is an empty string.</param>
    private static void AppendField(StringBuilder sb, string label, string value, int labelWidth = 9, string indent = "")
    {
        foreach (var line in WrapField(label, value, labelWidth, indent))
            sb.AppendLine(line);
    }

    #endregion

    public string ToLogString()
    {
        var fileName = string.IsNullOrEmpty(SourceFile) 
            ? "Unknown" 
            : Path.GetFileName(SourceFile);

        var location = $"{fileName}:{SourceLine} - {Caller}";
        var timestamp = Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

        var sb = new StringBuilder();

        sb.AppendLine($"[ {timestamp} ] [ {location} ] ");
        sb.AppendLine($"[ {Severity} ] - {Message}");

        if (Exception != null)
        {
            var current = Exception;
            var depth = 0;

            while (current != null && depth < 10)
            {
                var indent = new string(' ', depth * 2);
                
                sb.AppendLine($"{indent}Exception: {current.Type}");
                sb.AppendLine($"{indent}Message: {current.Message}");

                if (!string.IsNullOrEmpty(current.StackTrace))
                {
                    sb.AppendLine($"{indent}Stack: {current.StackTrace}");
                }
                
                current = current.Inner;
                depth++;
            }
        }

        sb.AppendLine();
        return sb.ToString();
    }
    
    /// <summary>
    /// Returns a formatted string representation of the log entry, including timestamp, severity, message, source
    /// information, and exception details if present.
    /// </summary>
    /// <remarks>The returned string is suitable for display in logs or diagnostic outputs. Exception
    /// information is included up to a depth of 10 inner exceptions. Source file and caller information are included if
    /// available.</remarks>
    /// <returns>A string containing the log entry in a structured, human-readable format. If an exception is associated with the
    /// entry, its details and stack trace are included.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine(BoxLine(Tl, Tr));
        AppendField(sb, "Timestamp", $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} UTC");
        AppendField(sb, "Severity", Severity.ToString());
        AppendField(sb, "Message", Message);
        sb.AppendLine(BoxLine(Ml, Mr));

        if (!string.IsNullOrEmpty(SourceFile))
        {
            AppendField(sb, "Source", $"{SourceFile}:{SourceLine}");

            if (!string.IsNullOrEmpty(Caller))
                AppendField(sb, "Caller", Caller);
        }

        if (Exception is null)
        {
            sb.AppendLine(BoxLine(Bl, Br));
            return sb.ToString();
        }

        sb.AppendLine(BoxContent(""));
        sb.AppendLine(BoxContent("Exception Details:"));

        var current = Exception;
        int depth = 0;

        while (current is not null && depth < 10)
        {
            string indent = new(' ', depth * 2);

            sb.AppendLine(BoxContent(""));
            AppendField(sb, "Type", current.Type, 7, indent);
            AppendField(sb, "Message", current.Message, 7, indent);
            sb.AppendLine(BoxContent($"{indent}Stack:"));

            if (string.IsNullOrEmpty(current.StackTrace))
            {
                sb.AppendLine(BoxContent($"{indent}  <No stack trace available>"));
            }
            else
            {
                foreach (var line in WrapText(current.StackTrace, indent + "  "))
                    sb.AppendLine(line);
            }

            current = current.Inner;
            depth++;

            if (current is not null)
            {
                sb.AppendLine(BoxContent(""));
                sb.AppendLine(BoxContent($"{indent}--- Inner Exception ---"));
            }
        }

        sb.AppendLine(BoxLine(Bl, Br));
        return sb.ToString();
    }
}

/*
 *------------------------------------------------------------
 * (MessageEnvelope.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */