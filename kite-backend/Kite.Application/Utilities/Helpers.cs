namespace Kite.Application.Utilities;

public static class Helpers
{
    public static string GetTimeElapsedString(DateTimeOffset requestTime)
    {
        var currentTime = DateTimeOffset.UtcNow;
        var timeSpan = currentTime - requestTime;

        return timeSpan switch
        {
            var ts when ts.TotalDays > 365 =>
                (int)(ts.TotalDays / 365) == 1
                    ? "1 year ago"
                    : $"{(int)(ts.TotalDays / 365)} years ago",

            var ts when ts.TotalDays > 30 =>
                (int)(ts.TotalDays / 30) == 1
                    ? "1 month ago"
                    : $"{(int)(ts.TotalDays / 30)} months ago",

            var ts when ts.TotalDays > 7 =>
                (int)(ts.TotalDays / 7) == 1
                    ? "1 week ago"
                    : $"{(int)(ts.TotalDays / 7)} weeks ago",

            var ts when ts.TotalDays >= 1 =>
                (int)ts.TotalDays == 1 ? "yesterday" : $"{(int)ts.TotalDays} days ago",

            var ts when ts.TotalHours >= 1 =>
                (int)ts.TotalHours == 1 ? "an hour ago" : $"{(int)ts.TotalHours} hours ago",

            var ts when ts.TotalMinutes >= 1 =>
                (int)ts.TotalMinutes == 1 ? "a minute ago" : $"{(int)ts.TotalMinutes} minutes ago",

            var ts when ts.TotalSeconds >= 30 => "less than a minute ago",

            _ => "just now"
        };
    }
}