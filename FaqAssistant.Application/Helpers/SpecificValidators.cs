using System.Text.RegularExpressions;

namespace FaqAssistant.Application.Helpers;

public static class SpecificValidators
{
    #region UserValidations
    public static bool IsValidEmail(string email)
    {
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }
    #endregion
}
