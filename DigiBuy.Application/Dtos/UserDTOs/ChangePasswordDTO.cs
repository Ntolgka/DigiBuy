namespace DigiBuy.Application.Dtos.UserDTOs;

public class ChangePasswordDTO
{
    public string UserId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}