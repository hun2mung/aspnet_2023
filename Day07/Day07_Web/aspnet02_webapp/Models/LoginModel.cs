using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace aspnet02_boardapp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "이메일 주소 필수!")]
        [EmailAddress]
        [Display(Name = "이메일주소")]
        public string Email { get; set; }

        [Required(ErrorMessage = "패스워드 필수!")]
        [DataType(DataType.Password)]
        [DisplayName("패스워드")]
        public string Password { get; set; }

        [DisplayName("이메일 주소 기억")]
        public bool RememberMe { get; set; }
    }
}
