using Moq;
using NUnit.Framework;
using TDD_examples.ExcessiveSetup;

namespace UnitTests.TheMockery
{
    public class UserCreationUnitTests
    {

        [Test]
        public void Validator_validates_correctly()
        {
            //Arrange
            var emailValidator = new Mock<IEmailValidator>();
            emailValidator
                .Setup(e => e.IsValid(It.IsAny<string>()))
                .Returns(true);

            var passwordValidator = new Mock<IPasswordChecker>();
            passwordValidator
                .Setup(e => e.IsValid(It.IsAny<string>()))
                .Returns(true);

            var userCreator = new UserCreator("", "", "", emailValidator.Object, passwordValidator.Object);

            //Act
            var isValid = userCreator.IsValid();

            //Assert
            Assert.IsTrue(isValid);
        }
    }
}
