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
                _ => userType
            };
        }
    }
}
