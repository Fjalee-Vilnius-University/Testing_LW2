using System;
using System.Linq;

namespace TDD_examples.ExcessiveSetup
{
    public class EmailValidator
    {
        private static readonly string _validSpecialChars = ".-@_";
        private static readonly string[] _validDomains = new string[] { "mif.stud.vu", "mif.vu", "vu", "gmail", "yahoo" };
        private static readonly string[] _validTLDs = new string[] { "lt", "com", "org" };

        public bool IsValid(string email)
        {
            return ValidEta(email) && ValidSymbols(email) && ValidDomainAndTLD(email);
        }

        private bool ValidEta(string email)
        {
            if (!email.Contains('@'))
                return false;

            if (email.Count(x => x == '@') >= 2)
                return false;

            return true;
        }

        private bool ValidSymbols(string email)
        {
            var illegalChars = email.ToCharArray()
                .Where(c => !Char.IsLetterOrDigit(c))
                .Where(c => !_validSpecialChars.Contains(c));

            if (illegalChars.Any())
                return false;

            return true;
        }

        private bool ValidDomainAndTLD(string email)
        {
            var emailEnding = email.Split('@').Last();

            if (!emailEnding.Contains('.'))
                return false;

            var domain = emailEnding.Substring(0, emailEnding.LastIndexOf('.'));
            var tld = emailEnding.Split('.').Last();

            if (!_validDomains.Contains(domain) || !_validTLDs.Contains(tld))
                return false;

            return true;
        }
    }
}
