using System.Text.RegularExpressions;
using Discord;

namespace Interactivity.Bot.Utilities;

public class DurationParser
{
    public class DurationParseException : Exception
    {
        public DurationParseException(string message) : base(message)
        {
            
        }
    }

    public class UnkownQuantifierException : DurationParseException
    {
        public readonly string Quantifier;
        
        public UnkownQuantifierException(String quantifier) : base($"Unknown quantifier {quantifier}")
        {
            Quantifier = quantifier;
        }
    }
    
    public static TimeSpan FromString(string input)
    {
        Regex matchQuantifiers = new Regex(@"(\d+)([a-zA-Z])");
        var matches = matchQuantifiers.Matches(input);

        int minutes = 0; // m
        int hours = 0;   // h
        int days = 0;    // d
        int weeks = 0;   // w
        int months = 0;  // M
        int years = 0;   // y

        foreach (Match match in matches)
        {
            var quantifier = match.Groups[2].Value;
            var value = Convert.ToInt32(match.Groups[1].Value);
            switch (quantifier)
            {
                case "m":
                    minutes += value;
                    break;
                case "h":
                    hours += value;
                    break;
                case "d":
                    days += value;
                    break;
                case "w":
                    weeks += value;
                    break;
                case "M":
                    months += value;
                    break;
                case "y":
                    years += value;
                    break;
                default:
                    throw new UnkownQuantifierException(quantifier);
            }
        }

        var span = new TimeSpan(days + (7 * weeks) + (31 * months) + (365 * years), hours, minutes, 0);
        return span;
    }

    public static EmbedBuilder QuantifierEmbed()
    {
        var builder = new EmbedBuilder()
            .WithTitle("Time span quantifiers:");
        builder.Description += "`m` - Minutes\n";
        builder.Description += "`h` - Hours\n";
        builder.Description += "`d` - Days\n";
        builder.Description += "`w` - Weeks\n";
        builder.Description += "`M` - Months\n";
        builder.Description += "`y` - Years\n";
        builder.Description += "\n**Note**: *These quantifiers are CASE SENSITIVE*";
        return builder;
    }
}