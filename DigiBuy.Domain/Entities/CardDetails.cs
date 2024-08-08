namespace DigiBuy.Domain.Entities;

public class CardDetails
{
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public string ExpiryMonth { get; set; } // Format: MM
    public string ExpiryYear { get; set; } // Format: YYYY
    public string CVV { get; set; } // 3 or 4 digits depending on the card type
}