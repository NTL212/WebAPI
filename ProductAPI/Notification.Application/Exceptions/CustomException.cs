
namespace Notification.Application.Exceptions
{

    // Custom Exception Class for handling specific service errors
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
        }
    }
}
