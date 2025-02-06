namespace LMS.Api.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public bool isAuthenticated { get; set; }
        public IEnumerable<string> Roles {  get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime ExpiresOn { get; set; }


    }
}
