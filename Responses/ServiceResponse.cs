namespace api.Responses
{
    public class ServiceResponse
    {
        public record class GeneralResponse(bool Flag, string Message, string token);
        public record class LoginResponse(bool Flag, string Message, string token);

    }
}
