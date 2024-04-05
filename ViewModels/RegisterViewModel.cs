using System.ComponentModel.DataAnnotations;

namespace MIL_LIT.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        public string Email {get; set; }
        
        [Required]
        [Display(Name ="Пароль")]
        public string Password {get; set;}

        [Required]
        [Compare("Password", ErrorMessage = "Паролі не збігаються")]
        [Display(Name = "Підтвердження паролю")]
        [DataType(DataType.Password)]
        public string PasswordConfirm {get; set;}
    }
}