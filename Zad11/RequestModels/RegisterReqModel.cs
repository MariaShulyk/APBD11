using System.ComponentModel.DataAnnotations;

namespace Zad11.Models;

public class RegisterReqModel
{
    [MaxLength(50)]
    public string Email { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid e-mail")]
    public string Login { get; set; }
    
    [MaxLength(50)]
    public string Password { get; set; }
}