using System.Globalization;

namespace CornerStore;

internal sealed class PromoRuleParser(string rule)
{
    private int position;

    public static PromoExpression Parse(string rule)
    {
        if (string.IsNullOrWhiteSpace(rule))
        {
            throw new FormatException("A promo rule cannot be empty.");
        }

        return new PromoRuleParser(rule).ParseRule();
    }

    private PromoExpression ParseRule()
    {
        SkipWhitespace();
        var expression = ParsePredicate();

        while (true)
        {
            var whitespace = SkipWhitespace();
            if (position == rule.Length)
            {
                return expression;
            }

            if (whitespace == 0 || !TryReadKeyword("AND"))
            {
                throw Error("Expected AND or the end of the rule");
            }

            if (position == rule.Length || !char.IsWhiteSpace(rule[position]))
            {
                throw Error("Expected whitespace after AND");
            }

            SkipWhitespace();
            expression = new AndExpression(expression, ParsePredicate());
        }
    }

    private PromoExpression ParsePredicate()
    {
        var name = ReadIdentifier();
        SkipWhitespace();
        if (position == rule.Length)
        {
            throw Error("Expected > or =");
        }

        var operation = rule[position++];
        SkipWhitespace();

        return (name, operation) switch
        {
            ("SPEND", '>') => new SpendGreaterThanExpression(ReadNumber()),
            ("QTY", '>') => new QuantityGreaterThanExpression(ReadNumber()),
            ("CATEGORY", '=') => new CategoryEqualsExpression(ReadIdentifier()),
            ("REGION", '=') => new RegionEqualsExpression(ReadIdentifier()),
            _ => throw Error($"Unsupported predicate {name}{operation}"),
        };
    }

    private string ReadIdentifier()
    {
        if (position == rule.Length || !IsIdentifierStart(rule[position]))
        {
            throw Error("Expected an identifier");
        }

        var start = position++;
        while (position < rule.Length && IsIdentifierPart(rule[position]))
        {
            position++;
        }

        return rule[start..position];
    }

    private decimal ReadNumber()
    {
        var start = position;
        while (position < rule.Length && char.IsAsciiDigit(rule[position]))
        {
            position++;
        }

        if (position < rule.Length && rule[position] == '.')
        {
            position++;
            var fractionalStart = position;
            while (position < rule.Length && char.IsAsciiDigit(rule[position]))
            {
                position++;
            }

            if (fractionalStart == position)
            {
                throw Error("Expected digits after the decimal point");
            }
        }

        if (start == position || !decimal.TryParse(
                rule.AsSpan(start, position - start),
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var value))
        {
            throw Error("Expected an invariant-culture number");
        }

        return value;
    }

    private bool TryReadKeyword(string keyword)
    {
        if (!rule.AsSpan(position).StartsWith(keyword.AsSpan(), StringComparison.Ordinal))
        {
            return false;
        }

        var end = position + keyword.Length;
        if (end < rule.Length && IsIdentifierPart(rule[end]))
        {
            return false;
        }

        position = end;
        return true;
    }

    private int SkipWhitespace()
    {
        var start = position;
        while (position < rule.Length && char.IsWhiteSpace(rule[position]))
        {
            position++;
        }

        return position - start;
    }

    private FormatException Error(string message)
    {
        return new FormatException($"Invalid promo rule '{rule}' at position {position}: {message}.");
    }

    private static bool IsIdentifierStart(char value)
    {
        return value is >= 'A' and <= 'Z'
            or >= 'a' and <= 'z';
    }

    private static bool IsIdentifierPart(char value)
    {
        return IsIdentifierStart(value)
            || char.IsAsciiDigit(value)
            || value is '_' or '-';
    }
}
