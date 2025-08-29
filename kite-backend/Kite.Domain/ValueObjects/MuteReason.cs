namespace Kite.Domain.ValueObjects;

public readonly record struct MuteReason(string Value)
{
    private static readonly MuteReason Spam = new("spam");
    private static readonly MuteReason Harassment = new("harassment");
    private static readonly MuteReason OffTopic = new("off-topic");
    private static readonly MuteReason Other = new("other");

    private static readonly HashSet<string> Allowed =
        new(StringComparer.OrdinalIgnoreCase)
        {
            Spam.Value, Harassment.Value, OffTopic.Value, Other.Value
        };

    public override string ToString() => Value;

    public static bool TryParse(string value, out MuteReason reason)
    {
        if (!string.IsNullOrWhiteSpace(value) && Allowed.Contains(value))
        {
            reason = new MuteReason(value);
            return true;
        }

        reason = default;
        return false;
    }
}