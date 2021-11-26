using System;
using System.Linq;

namespace TDD_examples.ExcessiveSetup
{
    public class PasswordChecker
    {
        private readonly int _minLength = 8;
        private readonly char[] _specialSymbols = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '<', '>', '?' };

        public bool IsValid(string password)
        {
            return ValidLength(password) && ContainsUppercase(password) && ContainsSpecialChar(password);
        }

        private bool ValidLength(string password)
        {
            return password.Length >= _minLength;
        }

        private bool ContainsUppercase(string password)
        {
            var upperLetters = password.ToCharArray()
                                .Where(c => Char.IsUpper(c));

            return upperLetters.Any();
        }

        private bool ContainsSpecialChar(string password)
        {
            var specialChars = password.ToCharArray()
                                .Where(c => _specialSymbols.Contains(c));

            return specialChars.Any();
        }
    }
}
