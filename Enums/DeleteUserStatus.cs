namespace GrapheneTrace.Enums;

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