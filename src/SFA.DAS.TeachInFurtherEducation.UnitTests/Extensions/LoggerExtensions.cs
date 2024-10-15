using FakeItEasy;
using FakeItEasy.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

public static class LoggerExtensions
{
    public static void VerifyLogMustHaveHappened<T>(this ILogger<T> logger, Microsoft.Extensions.Logging.LogLevel level, string message)
    {
        try
        {
            logger.VerifyLog(level, message)
                .MustHaveHappened();
        }
        catch (Exception e)
        {
            throw new ExpectationException($"while verifying a call to log with message: \"{message}\"", e);
        }
    }

    public static void VerifyLogMustNotHaveHappened<T>(this ILogger<T> logger, Microsoft.Extensions.Logging.LogLevel level, string message)
    {
        try
        {
            logger.VerifyLog(level, message)
                .MustNotHaveHappened();
        }
        catch (Exception e)
        {
            throw new ExpectationException($"while verifying a call to log with message: \"{message}\"", e);
        }
    }

    public static IVoidArgumentValidationConfiguration VerifyLog<T>(this ILogger<T> logger, Microsoft.Extensions.Logging.LogLevel level, string message)
    {
        return A.CallTo(logger)
            .Where(call => call.Method.Name == "Log"
                && call.GetArgument<Microsoft.Extensions.Logging.LogLevel>(0) == level
                && CheckLogMessages(call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2), message));
    }

    private static bool CheckLogMessages(IReadOnlyList<KeyValuePair<string, object>> readOnlyLists, string message)
    {
        // Get the original message
        var originalFormat = readOnlyLists.FirstOrDefault(k => k.Key =="{OriginalFormat}").Value?.ToString();

        if (originalFormat != null)
        {
            // Perform the substitutions
            foreach (var kvp in readOnlyLists)
            {
                if (kvp.Key != "{OriginalFormat}")
                {
                    originalFormat = originalFormat.Replace($"{{{kvp.Key}}}", kvp.Value.ToString());
                }
            }

            return (originalFormat.Contains(message));
        }
        else
        {
            foreach (var kvp in readOnlyLists)
            {
                if (kvp.Value.ToString().Contains(message))
                {
                    return true;
                }
            }

        }

        return false;
    }
}
