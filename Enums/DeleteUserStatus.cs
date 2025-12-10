// SID: 2408078
namespace GrapheneTrace.Enums
{
    /// <summary>
    /// Enum for the status of user deletion 
    /// </summary>
    public enum DeleteUserStatus
    {
        Success,
        UserNotFound,
        CannotDeleteSelf,
        DatabaseError
    }
}