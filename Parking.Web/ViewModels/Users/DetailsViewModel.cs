using System.ComponentModel.DataAnnotations;

namespace Parking.Web.ViewModels.Users
{
    public class DetailsViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Не указан номер телефона")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage = "Не указан электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        public string Email { get; set; }
    }
}
