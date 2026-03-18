using FluentValidation;
using RecipeMgt.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Validator.Csv
{
    public class CreateUserCsvDtoValidator : AbstractValidator<CreateUserCsvDto>
    {
        public CreateUserCsvDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("EMAIL_REQUIRED")
                .EmailAddress().WithMessage("EMAIL_INVALID");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("PASSWORD_REQUIRED")
                .MinimumLength(6).WithMessage("PASSWORD_MIN_6");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FULLNAME_REQUIRED")
                .MaximumLength(100).WithMessage("FULLNAME_MAX_100");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("ROLE_REQUIRED");
        }
    }
}
