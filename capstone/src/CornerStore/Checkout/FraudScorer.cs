namespace CornerStore;

internal class FraudScorer
{
    public int Score(Customer customer)
    {
        return new Random(Config.Instance.Seed + customer.Number).Next(0, 100);
    }
}