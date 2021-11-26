namespace TDD_examples.ExcessiveSetup
{
    public class UserCreator
    {
        private readonly string _email;
        private readonly string _password;
        private readonly IEmailValidator _emailValidator;
        private readonly IPasswordChecker _passwordValidator;

        public UserCreator(string userName, string password, string email, IEmailValidator emailValidator, IPasswordChecker passwordChecker)
        {
            _email = email;
            _password = password;
            _emailValidator = emailValidator;
            _passwordValidator = passwordChecker;
        }

        public bool IsValid()
        {
            var isEmailValid = _emailValidator.IsValid(_email);
            var isPasswordValid = _passwordValidator.IsValid(_password);

            return isEmailValid && isPasswordValid;
        }
    }
}
