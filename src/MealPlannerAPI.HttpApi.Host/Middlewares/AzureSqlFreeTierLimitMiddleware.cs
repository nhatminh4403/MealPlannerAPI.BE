using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MealPlannerAPI.HttpApi.Host.Middlewares;

public class AzureSqlFreeTierLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AzureSqlFreeTierLimitMiddleware> _logger;
    private static bool _databaseLimitReached = false;
    private static DateTime? _lastCheckTime = null;
    private static string _resetDateInfo = "soon";
    private static string _fullErrorMessage = "";
    private static string _currentMonth = "";
    private static string _nextMonth = "";
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

    public AzureSqlFreeTierLimitMiddleware(RequestDelegate next, ILogger<AzureSqlFreeTierLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // If we know the limit is reached, immediately show the error page
        if (_databaseLimitReached &&
            _lastCheckTime.HasValue &&
            DateTime.UtcNow - _lastCheckTime.Value < CheckInterval)
        {
            await ShowLimitReachedPage(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Check if it's a database limit exception
            if (IsAzureSqlLimitException(ex, out string resetDate, out string errorMessage, out string currentMonth, out string nextMonth))
            {
                _databaseLimitReached = true;
                _lastCheckTime = DateTime.UtcNow;
                _resetDateInfo = resetDate;
                _fullErrorMessage = errorMessage;
                _currentMonth = currentMonth;
                _nextMonth = nextMonth;

                _logger.LogWarning("Database limit reached. Reset date: {ResetDate}", resetDate);

                if (!context.Response.HasStarted)
                {
                    await ShowLimitReachedPage(context);
                }
                return;
            }

            throw;
        }
    }

    private bool IsAzureSqlLimitException(Exception ex, out string resetDate, out string errorMessage, out string currentMonth, out string nextMonth)
    {
        resetDate = "soon";
        errorMessage = "";
        currentMonth = DateTime.UtcNow.ToString("MMMM yyyy");
        nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MMMM yyyy");

        var exception = ex;
        while (exception != null)
        {
            if (exception is SqlException sqlEx)
            {
                errorMessage = sqlEx.Message;

                // Azure SQL Database free tier limit error code
                if (sqlEx.Number == 42119 || sqlEx.Number == 40613 || sqlEx.Number == 40501 ||
                    sqlEx.Number == 49918 || sqlEx.Number == 10928 ||
                    sqlEx.Number == 10929)
                {
                    // Extract reset date and month info from error message
                    // Pattern: "for the month of December 2025"
                    var monthPattern = @"for the month of ([A-Za-z]+\s+\d{4})";
                    var monthMatch = Regex.Match(sqlEx.Message, monthPattern);

                    if (monthMatch.Success)
                    {
                        currentMonth = monthMatch.Groups[1].Value;
                    }

                    // Extract reset date from error message
                    // Pattern: "at 12:00 AM (UTC) on January 01, 2026"
                    var datePattern = @"at\s+(\d{1,2}:\d{2}\s+[AP]M)\s+\(UTC\)\s+on\s+([A-Za-z]+\s+\d{1,2},\s+\d{4})";
                    var match = Regex.Match(sqlEx.Message, datePattern);

                    if (match.Success)
                    {
                        var time = match.Groups[1].Value;
                        var date = match.Groups[2].Value;
                        resetDate = $"{date} at {time} UTC";

                        // Extract next month from reset date
                        var nextMonthPattern = @"([A-Za-z]+)\s+\d{1,2},\s+(\d{4})";
                        var nextMonthMatch = Regex.Match(date, nextMonthPattern);
                        if (nextMonthMatch.Success)
                        {
                            nextMonth = $"{nextMonthMatch.Groups[1].Value} {nextMonthMatch.Groups[2].Value}";
                        }

                        // Try to parse the full date and calculate time remaining
                        if (DateTime.TryParse($"{date} {time}", out DateTime parsedDate))
                        {
                            var timeUntilReset = parsedDate - DateTime.UtcNow;
                            if (timeUntilReset.TotalDays > 1)
                            {
                                resetDate = $"{date} at {time} UTC (in {(int)timeUntilReset.TotalDays} days)";
                            }
                            else if (timeUntilReset.TotalHours > 1)
                            {
                                resetDate = $"{date} at {time} UTC (in {(int)timeUntilReset.TotalHours} hours)";
                            }
                            else if (timeUntilReset.TotalMinutes > 0)
                            {
                                resetDate = $"{date} at {time} UTC (in {(int)timeUntilReset.TotalMinutes} minutes)";
                            }
                        }
                    }
                    else
                    {
                        // Fallback: try to find any date mention
                        var simpleDatePattern = @"([A-Za-z]+\s+\d{1,2},\s+\d{4})";
                        var simpleMatch = Regex.Match(sqlEx.Message, simpleDatePattern);
                        if (simpleMatch.Success)
                        {
                            resetDate = simpleMatch.Groups[1].Value;
                        }
                    }

                    return true;
                }

                // Check error message for quota/limit keywords
                if (sqlEx.Message.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                    sqlEx.Message.Contains("free amount allowance", StringComparison.OrdinalIgnoreCase) ||
                    sqlEx.Message.Contains("DTU", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            exception = exception.InnerException;
        }

        return false;
    }

    private async Task ShowLimitReachedPage(HttpContext context)
    {
        context.Response.StatusCode = 503; // Service Unavailable
        context.Response.ContentType = "text/html; charset=utf-8";

        var html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Database Limit Reached</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif; display: flex; justify-content: center; align-items: center; min-height: 100vh; background: #667eea; padding: 20px; }}
        .container {{ background: white; padding: 3rem; border-radius: 15px; box-shadow: 0 20px 60px rgba(0,0,0,0.3); text-align: center; max-width: 600px; width: 100%; }}
        .emoji {{ font-size: 5rem; margin-bottom: 1.5rem; animation: bounce 2s infinite; }}
        @keyframes bounce {{ 0%, 100% {{ transform: translateY(0); }} 50% {{ transform: translateY(-20px); }} }}
        h1 {{ color: #667eea; margin-bottom: 1rem; font-size: 2rem; font-weight: 700; }}
        .subtitle {{ color: #e74c3c; font-size: 1.1rem; font-weight: 600; margin-bottom: 1.5rem; }}
        p {{ color: #555; line-height: 1.8; margin-bottom: 1.5rem; font-size: 1rem; }}
        .casual-text {{ font-size: 1.05rem; color: #444; font-style: italic; }}
        .info-box {{ background: #f5576c; color: white; padding: 1.5rem; border-radius: 10px; margin: 1.5rem 0; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }}
        .info-box strong {{ font-size: 1.2rem; display: block; margin-bottom: 0.5rem; }}
        .reset-date {{ font-size: 1.3rem; font-weight: bold; color: #fff; background: rgba(0,0,0,0.2); padding: 0.8rem; border-radius: 8px; margin-top: 1rem; }}
        .details {{ background: #f8f9fa; padding: 1.5rem; border-radius: 10px; margin-top: 1.5rem; border-left: 5px solid #667eea; text-align: left; }}
        .details h3 {{ color: #667eea; margin-bottom: 1rem; font-size: 1.2rem; }}
        .details p {{ color: #666; font-size: 0.95rem; margin-bottom: 0.8rem; }}
        .icon {{ display: inline-block; margin-right: 0.5rem; }}
        .footer {{ margin-top: 2rem; padding-top: 1.5rem; border-top: 2px solid #eee; color: #999; font-size: 0.9rem; }}
        .student-badge {{ display: inline-block; background: #ffd700; color: #333; padding: 0.5rem 1rem; border-radius: 20px; font-size: 0.85rem; font-weight: bold; margin-bottom: 1rem; }}
        .error-details {{ background: #fff3cd; border: 1px solid #ffc107; padding: 1rem; border-radius: 8px; margin-top: 1rem; text-align: left; font-size: 0.85rem; color: #856404; max-height: 200px; overflow-y: auto; }}
        .error-details pre {{ white-space: pre-wrap; word-wrap: break-word; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='emoji'>💸</div>
        <div class='student-badge'>👨‍🎓 Student Project on Azure Free Tier</div>
        <h1>Database Limit Reached</h1>
        <p class='subtitle'>We've reached our database quota for {_currentMonth}</p>
        
        <p class='casual-text'>
            This application is running on an Azure SQL Database free tier, which provides a limited amount of resources for {_currentMonth}.
        </p>
        
        <p class='casual-text'>
            The database will automatically resume and become available again in {_nextMonth}.
        </p>
 
        <div class='info-box'>
            <strong><span class='icon'>⏰</span>Resumes On</strong>
            <div class='reset-date'>{_resetDateInfo}</div>
        </div>
 
        <div class='details'>
            <h3><span class='icon'>🎓</span>Why did this happen?</h3>
            <p><strong>• </strong>The free tier allows a maximum of 100,000 vCore seconds per month.</p>
            <p><strong>• </strong>Due to high traffic or complex queries, we've exhausted this allowance.</p>
            <p><strong>• </strong>To prevent unexpected charges, Azure automatically pauses the database.</p>
            <p><strong>• </strong>No data is lost; it's simply inaccessible until the limits reset.</p>
        </div>
 
        {(string.IsNullOrEmpty(_fullErrorMessage) ? "" : $@"
        <details style='margin-top: 1.5rem;'>
            <summary style='cursor: pointer; color: #667eea; font-weight: bold; padding: 0.5rem;'>
                🔍 Technical Details
            </summary>
            <div class='error-details'>
                <pre>{System.Net.WebUtility.HtmlEncode(_fullErrorMessage)}</pre>
            </div>
        </details>
        ")}
 
        <div class='footer'>
            <p>We apologize for the inconvenience.</p>
            <p><strong>Please come back in {_nextMonth} when the database quota resets.</strong></p>
            <p style='margin-top: 1rem; font-size: 0.8rem;'>
                If you are the administrator, you can upgrade the database tier in the Azure Portal to restore access immediately.
            </p>
        </div>
    </div>
</body>
</html>";

        await context.Response.WriteAsync(html);
    }
}

public static class AzureSqlFreeTierLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseAzureSqlFreeTierLimit(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AzureSqlFreeTierLimitMiddleware>();
    }
}
