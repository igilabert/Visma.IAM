using Visma.IAM.Models;
using Xunit;

namespace Visma.IAM.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_CanBeCreated()
        {
            var user = new User();

            Assert.NotNull(user);
        }
    }
}
