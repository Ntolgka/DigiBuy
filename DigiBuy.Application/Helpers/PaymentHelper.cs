using System.Text.RegularExpressions;
using DigiBuy.Domain.Entities;

namespace DigiBuy.Application.Helpers;

public static class PaymentHelper
{
    public static bool ValidateCard(CardDetails cardDetails)
    {
        return IsCardNumberValid(cardDetails.CardNumber) &&
               IsCardHolderNameValid(cardDetails.CardHolderName) &&
               IsExpiryDateValid(cardDetails.ExpiryMonth, cardDetails.ExpiryYear) &&
               IsCvvValid(cardDetails.CVV);
    }

    private static bool IsCardNumberValid(string cardNumber)
    {
        var regex = new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$"              // Visa
                              + @"|^5[1-5][0-9]{14}$"                   // MasterCard
                              + @"|^3[47][0-9]{13}$"                    // American Express
                              + @"|^6(?:011|5[0-9]{2})[0-9]{12}$"       // Discover
                              + @"|^3(?:0[0-5]|[68][0-9])[0-9]{11}$"    // Diners Club
                              + @"|^35(?:[0-9]{14})$");                 // JCB

        return regex.IsMatch(cardNumber);
    }

    private static bool IsCardHolderNameValid(string cardHolderName)
    {
        var regex = new Regex(@"^[A-Za-z\s]+$"); // Validates that the name contains only letters and spaces
        return regex.IsMatch(cardHolderName);
    }

    private static bool IsExpiryDateValid(string expiryMonth, string expiryYear)
    {
        var monthRegex = new Regex(@"^(0[1-9]|1[0-2])$"); // Validates month is between 01 and 12
        var yearRegex = new Regex(@"^\d{4}$"); // Validates year is 4 digits

        if (!monthRegex.IsMatch(expiryMonth) || !yearRegex.IsMatch(expiryYear))
        {
            return false;
        }

        // Check if the expiry date is in the future
        var expiryDate = new DateTime(int.Parse(expiryYear), int.Parse(expiryMonth), 1).AddMonths(1).AddDays(-1);
        return DateTime.UtcNow <= expiryDate;
    }

    private static bool IsCvvValid(string cvv)
    {
        var regex = new Regex(@"^\d{3,4}$"); // Validates CVV is exactly 3 or 4 digits (for American Express)
        return regex.IsMatch(cvv);
    }
}