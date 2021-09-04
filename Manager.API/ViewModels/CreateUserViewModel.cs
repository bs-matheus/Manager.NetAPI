using System.ComponentModel.DataAnnotations;

namespace Manager.API.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "O nome n�o pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O nome deve ter no m�nimo 3 caracteres.")]
        [MaxLength(80, ErrorMessage = "O nome deve ter no m�ximo 80 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O email n�o pode ser vazio.")]
        [MinLength(10, ErrorMessage = "O email deve ter no m�nimo 10 caracteres.")]
        [MaxLength(180, ErrorMessage = "O email deve ter no m�ximo 180 caracteres.")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                        ErrorMessage = "O email informado n�o � v�lido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha n�o pode ser vazia.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no m�nimo 6 caracteres.")]
        [MaxLength(30, ErrorMessage = "A senha deve ter no m�ximo 30 caracteres.")]
        public string Password { get; set; }
    }
}
