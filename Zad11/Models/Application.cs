using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zad11.Models;

[Table("Users")]
public class Application
{
    [Key]
    [Column("Id")]
    public int IdUser { get; set; }
        
    [Column("Login")] 
    public string Login { get; set; }
        
    [Column("Email")]
    public string Email { get; set; }
        
    [Column("Password")]
    public string Password { get; set; }
        
    [Column("Salt")]
    public string Salt { get; set; }
        
    [Column("RefreshToken")] 
    public string RefreshToken { get; set; }
        
    [Column("RefreshTokenExp")]
    public DateTime? RefreshTokenExp { get; set; }
}