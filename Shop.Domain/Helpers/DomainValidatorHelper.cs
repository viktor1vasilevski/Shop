using Shop.Domain.Exceptions;

namespace Shop.Domain.Helpers;

public static class DomainValidatorHelper
{
    public static void ThrowIfNullOrWhiteSpace(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException($"{propertyName} cannot be empty.");
    }

    public static void ThrowIfTooLong(string value, int maxLength, string propertyName)
    {
        if (value.Length > maxLength)
            throw new DomainValidationException($"{propertyName} cannot exceed {maxLength} characters.");
    }
}
