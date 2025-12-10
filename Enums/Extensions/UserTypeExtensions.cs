// SID: 2408078
namespace GrapheneTrace.Enums.Extensions
{
    public static class UserTypeExtensions
    {
        public static UserType Opposite(this UserType userType)
        {
            return userType switch
            {
                UserType.Patient => UserType.Clinician,
                UserType.Clinician => UserType.Patient,
                UserType.Admin => throw new ArgumentException("User Type does not have an opposite"),
                _ => throw new ArgumentException("User Type does not have an opposite")
            };
        }
    }
}
