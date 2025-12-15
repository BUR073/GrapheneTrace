// SID: 2408078
namespace GrapheneTrace.Enums.Extensions
{
    public static class UserTypeExtensions
    {
        /// <summary>
        /// Return the opposite user type
        /// Patient --> Clinician
        /// Clinician --> Patient
        /// </summary>
        /// <param name="userType"></param> The user type to oppose 
        /// <returns></returns> The opposite user type
        /// <exception cref="ArgumentException"></exception> If Admin, there is no opposite
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
