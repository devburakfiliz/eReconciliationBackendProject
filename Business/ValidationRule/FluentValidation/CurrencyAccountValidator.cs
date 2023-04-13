using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRule.FluentValidation
{
    public class CurrencyAccountValidator : AbstractValidator<CurrencyAccount>
    {
        public CurrencyAccountValidator()
        {
            RuleFor(p=> p.Name).NotEmpty().WithMessage("Firma Adı Boş olamaz");
            RuleFor(p=> p.Address).NotEmpty().WithMessage("Firma Adı Boş olamaz");
            RuleFor(p=> p.Name).MinimumLength(4).WithMessage("Firma Adı 4 karakterden az olamaz");
            RuleFor(p=> p.Address).MinimumLength(4).WithMessage("Firma Adı 4 karakterden az olamaz");
        }
    }
}
