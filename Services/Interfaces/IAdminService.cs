using System.Collections.Generic;
using GrapheneTrace.Models.Database;
using GrapheneTrace.Models.Admin;
using GrapheneTrace.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace GrapheneTrace.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> UpdateUser(EditUserViewModel model);

        Task UpdatePatientClinicianLinks(List<int> idsToAdd, List<int> idsToRemove, int primaryUserId,
            bool isManagingPatient);

        Task<List<int>> GetAlreadyLinkedUsers(int Id, string type);

        Task<IList<SelectListItem>> ManagePatientGetLinks(IList<ApplicationUser> allClinicians,
            List<int> alreadyLinkedClinicianIds, string type);

        Task<IList<SelectListItem>> ManageClinicianGetLinks(IList<ApplicationUser> allPatients,
            List<int> alreadyLinkedPatientIds, string type);
    }
}