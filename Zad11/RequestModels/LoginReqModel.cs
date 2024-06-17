using System.ComponentModel.DataAnnotations;

namespace Zad11.Models;

public class LoginReqModel
{
    [MaxLength(50)]
    public string Login { get; set; }
    
    [MaxLength(50)]
    public string Password { get; set; }
}