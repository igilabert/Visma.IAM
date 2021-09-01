namespace Visma.IAM.Dto
{
    public class UserDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public string Password { get; set; } // should be a hash instead of plain password
    }
}
