namespace Armut.Messaging.Application.DTOs
{
    public class AuthResponse
    {
        public AuthResponse(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
