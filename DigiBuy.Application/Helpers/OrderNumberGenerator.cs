namespace DigiBuy.Application.Helpers;

public static class OrderNumberGenerator
{
    private static readonly Random random = new Random();

    public static string GenerateOrderNumber()
    {
        var timestampPart = DateTime.UtcNow.ToString("yyMMddHH");
        var randomPart = random.Next(100000, 999999).ToString();
        var orderNumber = $"{timestampPart}{randomPart}".Substring(0, 9);
        return orderNumber;
    }
}