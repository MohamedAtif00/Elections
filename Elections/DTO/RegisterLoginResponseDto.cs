namespace Elections.DTO
{
    public class RegisterLoginResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }

        public string Role { get; set; }  // Add the Role field 
        public int Id { get; set; }
    }
}
