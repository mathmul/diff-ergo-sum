namespace DiffErgoSum.Api.Validation;

using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;

using DiffErgoSum.Core.Constants;

public class ValidBase64Attribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not string s)
            return ValidationResult.Success; // [Required] handles null/empty checks

        if (Base64.IsValid(s))
            return ValidationResult.Success;

        return new ValidationResult(ValidationMessages.InvalidBase64);
    }
}
