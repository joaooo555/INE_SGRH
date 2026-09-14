using System.ComponentModel.DataAnnotations;

namespace SGRH.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [Display(Name = "Nome Completo")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password é obrigatória")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A password deve ter pelo menos 6 caracteres")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As passwords não coincidem")]
        [Display(Name = "Confirmar Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Deve aceitar os termos e condições")]
        [Display(Name = "Aceito os termos e condições")]
        public bool AcceptTerms { get; set; }
    }
}
