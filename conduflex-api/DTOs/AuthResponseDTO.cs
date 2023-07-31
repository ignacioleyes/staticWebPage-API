namespace conduflex_api.DTOs
{
    public class AuthResponseDTO
    {
        public TokenDTO AuthToken { get; set; } = new TokenDTO();
        public string TokenType { get; set; }
        public ApplicationUserDTO AuthState { get; set; }
        public ApiError Error { get; set; }
    }

    public class TokenDTO
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
    
    public class ApiError
    {
        public string Description { get; set; }
        public string Code { get; set; }
    }
}
