using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class ApplicationUserDTO
    {
        private string? _fullName;
        private string? _phoneNumber;
        private string? _gender;
        private string? _address;

        [RegularExpression(@"^[a-zA-Z\s]{5,30}$", ErrorMessage = "Invalid format.")]
        public string? FullName
        {
            get => _fullName;
            set => _fullName = value?.Trim();
        }

        [RegularExpression("^(0[35789][0-9]{8}$|\\+84[35789][0-9]{8}$)", ErrorMessage = "Invalid format")]
        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value?.Trim();
        }

        [RegularExpression("^(nam|nữ|Nam|Nữ)$", ErrorMessage = "Invalid format.")]
        public string? Gender
        {
            get => _gender;
            set => _gender = value?.Trim();
        }

        [RegularExpression(@"^[a-zA-Z0-9\s,\/\-\u00C0-\u024F\u1E00-\u1EFF]{5,100}$", ErrorMessage = "Invalid format.")]
        public string? Address
        {
            get => _address;
            set => _address = value?.Trim();
        }
    }
}
