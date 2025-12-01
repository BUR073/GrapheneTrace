using GrapheneTrace.Models.Admin;
using GrapheneTrace.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrapheneTrace.Enums;
using GrapheneTrace.Models;

namespace GrapheneTrace.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> UpdateUser(EditUserViewModel model);

        Task UpdatePatientClinicianLinks(List<int> idsToAdd, List<int> idsToRemove, int primaryUserId,
            bool isManagingPatient);
        Task<List<int>> GetAlreadyLinkedUsers(int id, UserType type);
        IList<SelectListItem> GetLinkSelectionList(IList<ApplicationUser> allClinicians,
            List<int> alreadyLinkedClinicianIds, LinkFilter type);
        Task<List<UserViewModel>> GetAdminDashboardUsersAsync(string searchString);
    }
}

