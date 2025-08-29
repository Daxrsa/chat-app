namespace Kite.Domain.ValueObjects;

public readonly record struct BanReason(string Value)
{
    private static readonly BanReason Spam = new("spam");
    private static readonly BanReason Harassment = new("harassment");
    private static readonly BanReason HateSpeech = new("hate-speech");
    private static readonly BanReason OffTopic = new("off-topic");
    private static readonly BanReason Scam = new("scam");
    private static readonly BanReason Malware = new("malware");
    private static readonly BanReason Other = new("other");

    private static readonly HashSet<string> Allowed =
        new(StringComparer.OrdinalIgnoreCase)
        {
            Spam.Value,
            Harassment.Value,
            HateSpeech.Value,
            OffTopic.Value,
            Scam.Value,
            Malware.Value,
            Other.Value
        };

    public static IReadOnlyCollection<BanReason> All => new[]
    {
        Spam, Harassment, HateSpeech, OffTopic, Scam, Malware, Other
    };

    public override string ToString() => Value;

    public static bool TryParse(string value, out BanReason reason)
    {
        if (!string.IsNullOrWhiteSpace(value) && Allowed.Contains(value))
        {
            reason = new BanReason(value);
            return true;
        }

        reason = default;
        return false;
    }

    public static bool IsDefined(string value) =>
        !string.IsNullOrWhiteSpace(value) && Allowed.Contains(value);
}