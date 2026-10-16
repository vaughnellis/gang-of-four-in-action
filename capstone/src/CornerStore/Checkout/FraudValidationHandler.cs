using System.Globalization;

namespace CornerStore;

internal sealed class FraudValidationHandler(FraudScorer fraudScorer, OrderValidationHandler? next)
    : OrderValidationHandler(next)
{
    protected override string? Validate(OrderValidationContext context)
    {
        var fraudScore = fraudScorer.Score(context.Customer);
        return fraudScore > Config.Instance.FraudThreshold
            ? "fraud score " + fraudScore.ToString(CultureInfo.InvariantCulture) + " exceeds threshold"
            : null;
    }
}