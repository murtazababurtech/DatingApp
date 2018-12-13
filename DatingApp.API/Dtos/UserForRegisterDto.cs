using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required(AllowEmptyStrings = false,ErrorMessage="Bu alanı boş bırakmayınız")]
        public string UserName { get; set; }

         [Required(AllowEmptyStrings = false,ErrorMessage="Bu alanı boş bırakmayınız")]
         [StringLength(8,MinimumLength=4,ErrorMessage="Parola en az 4 ve en fazla 8 karakter olmalidi.r")]
        public string Password { get; set; }
    }
}