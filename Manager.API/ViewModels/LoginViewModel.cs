using System.ComponentModel.DataAnnotations;

namespace Manager.API.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O login n�o pode ser vazio.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A senha n�o pode ser vazia.")]
        public string Password { get; set; }
    }
}
