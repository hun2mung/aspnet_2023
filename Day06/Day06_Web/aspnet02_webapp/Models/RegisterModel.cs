using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aspnet02_boardapp.Models
{
    // 회원가입 할 때 데이터 받는 부분
    public class RegisterModel
    {
        [Required(ErrorMessage = "이메일 주소 필수!")]
        [EmailAddress]
        [Display(Name = "이메일주소")]
        public string Email { get; set; }

        [DisplayName("핸드폰 번호")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "패스워드 필수!")]
        [DataType(DataType.Password)]
        [DisplayName("패스워드")]
        public string Password { get; set; }

        [Required(ErrorMessage = "패스워드 확인 필수!")]
        [DataType(DataType.Password)]
        [DisplayName("패스워드 확인")]
        [Compare("Password", ErrorMessage = "패스워드 불일치! 재입력 바랍니다.")]
        public string ConfirmPassword { get; set; }
    }
}
