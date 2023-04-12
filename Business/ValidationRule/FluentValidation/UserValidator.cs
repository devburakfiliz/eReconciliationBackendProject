using Core.Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRule.FluentValidation
{
    public class UserValidator :AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(p=>p.Name).NotEmpty().WithMessage("Kullanıcı Adı Boş olamaz");
            RuleFor(p=>p.Name).MinimumLength(4).WithMessage("Kullanıcı Adı En az 4 karakter olmalıdır");
            RuleFor(p=>p.Email).NotEmpty().WithMessage("Mail Boş olamaz");
            RuleFor(p=>p.Email).EmailAddress().WithMessage("Lütfen Bir Email Giriniz.");
        }
    }
}
