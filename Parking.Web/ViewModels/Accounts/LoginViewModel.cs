using System.ComponentModel.DataAnnotations;

namespace Parking.Web.ViewModels.Accounts
{
    public class LoginViewModel
    {
        public string Role { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Не указан электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }
}
