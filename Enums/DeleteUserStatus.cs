namespace GrapheneTrace.Enums;

public enum DeleteUserStatus
{
    Success,
    UserNotFound,
    CannotDeleteSelf,
    DatabaseError
}