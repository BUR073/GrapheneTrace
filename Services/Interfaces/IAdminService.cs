using GrapheneTrace.Models.Admin;
using GrapheneTrace.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrapheneTrace.Enums;
using GrapheneTrace.Models;
using Microsoft.AspNetCore.Identity;

namespace GrapheneTrace.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> UpdateUser(EditUserViewModel model);

        Task UpdatePatientClinicianLinks(List<int> idsToAdd, List<int> idsToRemove, int primaryUserId,
            bool isClinician);
        Task<List<int>> GetAlreadyLinkedUsers(int id, UserType type);
        Task<IList<SelectListItem>> GetLinkSelectionList(UserType userType, int userId, LinkFilter type);
        Task<List<UserViewModel>> GetAdminDashboardUsersAsync(string searchString);
        Task<IdentityResult> CreateUser(CreateUserViewModel model);
    }
}

