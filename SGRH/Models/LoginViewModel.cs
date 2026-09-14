using System.ComponentModel.DataAnnotations;

namespace SGRH.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O utilizador é obrigatório")]
        [Display(Name = "Utilizador")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }
    }
}