public struct AdRevenue
{
    public decimal Revenue;
    public string Currency;

    public AdRevenue(decimal revenue, string currency)
    {
        Revenue = revenue;
        Currency = currency;
    }
}
