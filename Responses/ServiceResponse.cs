namespace api.Responses
{
    public class ServiceResponse
    {
        public record class GeneralResponse(bool Flag, string Message, string? Token);
        public record class LoginResponse(bool Flag, string Message, string? Token);

    }
}
