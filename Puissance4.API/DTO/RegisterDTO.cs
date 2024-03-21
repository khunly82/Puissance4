using System.ComponentModel.DataAnnotations;

namespace Puissance4.API.DTO
{
    public class RegisterDTO
    {
        [MaxLength(100)]
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
