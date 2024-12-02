using FluentValidation;
using Notification.Application.DTOs;


namespace Notification.Application.Validators
{
    public class OrderNoticeDTOValidator : AbstractValidator<OrderNoticeDTO>
    {
        public OrderNoticeDTOValidator()
        {
            // Validate Title: không được trống và độ dài từ 3 đến 200 ký tự
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 200).WithMessage("Title must be between 3 and 200 characters.");

            // Validate UserId: phải lớn hơn 0
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            // Validate OrderId: phải lớn hơn 0
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("OrderId must be greater than 0.");

            // Validate Message: không được trống và độ dài từ 5 đến 500 ký tự
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .Length(5, 500).WithMessage("Message must be between 5 and 500 characters.");

            // Validate Created: phải là một ngày hợp lệ và không quá trong tương lai
            RuleFor(x => x.Created)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Created date cannot be in the future.");

            // Validate IsSeen: mặc định không cần xác thực nhưng có thể thêm nếu cần thiết
            RuleFor(x => x.IsSeen)
                .Must(x => x == false || x == true).WithMessage("IsSeen must be a boolean value.");
        }
    }
}
