using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace NenTools.IO.Streams;

/// <summary>
/// LoggerWriter wrapping a Logger into a <see cref="TextWriter"/>.
/// </summary>
public class LoggerWriter : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    private ILogger _logger;
    public ILogger Logger => _logger;

    public LogLevel Level { get; }

    public LoggerWriter(ILogger logger, LogLevel level)
    {
        ArgumentNullException.ThrowIfNull(_logger, nameof(_logger));

        _logger = logger;
        Level = level;
    }


    public override void Write(char[] buffer, int index, int count)
    {
        Write(buffer.AsSpan(index, count).ToString());
    }

    public override void Write(string? value)
    {
        if (value != null)
            _logger.Log(Level, value, []);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
