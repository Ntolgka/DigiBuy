using DigiBuy.Domain.Entities;

namespace DigiBuy.Application.Helpers;

public static class PointsHelper
{
    public static decimal CalculatePointsToUse(User user, decimal amount)
    {
        return user.PointsBalance >= amount ? amount : user.PointsBalance;
    }

    public static void DeductPoints(User user, decimal pointsUsed)
    {
        user.PointsBalance -= pointsUsed;
    }
}