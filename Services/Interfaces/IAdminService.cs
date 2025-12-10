// SID: 2408078
using GrapheneTrace.Models.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrapheneTrace.Enums;
using Microsoft.AspNetCore.Identity;

namespace GrapheneTrace.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> UpdateUser(EditUserViewModel model);
        Task UpdatePatientClinicianLinks(List<int> idsToAdd, List<int> idsToRemove, int primaryUserId, UserType primaryUserType);
        Task<List<int>> GetAlreadyLinkedUsers(int id, UserType type);
        Task<IList<SelectListItem>> GetLinkSelectionList(UserType userType, int userId, LinkFilter type);
        Task<List<UserViewModel>> GetAdminDashboardUsersAsync(string searchString);
        Task<IdentityResult> CreateUser(CreateUserViewModel model);
        Task<DeleteUserStatus> DeleteUserAsync(int targetUserId, int currentId);
    }
}


