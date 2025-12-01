using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Services.Interfaces;
using GrapheneTrace.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrapheneTrace.Data; 
using GrapheneTrace.Models.Database; 



namespace GrapheneTrace.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        

        public AdminService(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole<int>> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> UpdateUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
                return false;

            // Update user fields
            user.Email = model.Email;
            user.UserName = model.Email;
            user.DateOfBirth = model.DateOfBirth;
            user.Name = model.Name;

            // Update roles
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.AddToRolesAsync(user, model.SelectedRoles.Except(userRoles));
            await _userManager.RemoveFromRolesAsync(user, userRoles.Except(model.SelectedRoles));

            // Update password if needed
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                await _userManager.RemovePasswordAsync(user);
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                    return false;
            }

            await _userManager.UpdateAsync(user);
            return true;
        }


        public async Task UpdatePatientClinicianLinks(List<int> idsToAdd, List<int> idsToRemove, int primaryUserId, bool isManagingPatient)
        {
            foreach (var idToAdd in idsToAdd)
            {
                var newLink = isManagingPatient
                    ? new PatientClinician { PatientId = primaryUserId, ClinicianId = idToAdd }
                    : new PatientClinician { PatientId = idToAdd, ClinicianId = primaryUserId };
                
                _context.PatientClinician.Add(newLink);
            }
        
            if (idsToRemove.Any())
            {
                IQueryable<PatientClinician> linksToRemoveQuery;
                
                linksToRemoveQuery = _context.PatientClinician
                    .Where(pc => pc.ClinicianId == primaryUserId && idsToRemove.Contains(pc.PatientId));
                
                if (isManagingPatient)
                {
                    linksToRemoveQuery = _context.PatientClinician
                        .Where(pc => pc.PatientId == primaryUserId && idsToRemove.Contains(pc.ClinicianId));
                }
                
                var linksToRemove = await linksToRemoveQuery.ToListAsync();
                _context.PatientClinician.RemoveRange(linksToRemove);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetAlreadyLinkedUsers(int Id, string type)
        {
            if (type == "Clinician")
            {
                return await _context.PatientClinician
                    .Where(pc => pc.PatientId == Id)
                    .Select(pc => pc.ClinicianId)
                    .ToListAsync();
            }

            if (type == "Patient")
            {
                return await _context.PatientClinician
                    .Where(pc => pc.ClinicianId == Id)
                    .Select(pc => pc.PatientId)
                    .ToListAsync();
            }

            return new List<int>();
            
        }



        public Task<IList<SelectListItem>> ManagePatientGetLinks(IList<ApplicationUser> allClinicians,
            List<int> alreadyLinkedClinicianIds, string type)
        {
            var links = type switch
            {
                "Assigned" => allClinicians
                    .Where(c => alreadyLinkedClinicianIds.Contains(c.Id))
                    .Select(c => new SelectListItem
                    {
                        Text = $"{c.Name} ({c.Email})",
                        Value = c.Id.ToString()
                    })
                    .ToList(),

                "Available" => allClinicians
                    .Where(c => !alreadyLinkedClinicianIds.Contains(c.Id))
                    .Select(c => new SelectListItem
                    {
                        Text = $"{c.Name} ({c.Email})",
                        Value = c.Id.ToString()
                    }).ToList(),

                _ => new List<SelectListItem>(),


            };
            return Task.FromResult<IList<SelectListItem>>(links);
        }

        public Task<IList<SelectListItem>> ManageClinicianGetLinks(IList<ApplicationUser> allPatients,
            List<int> alreadyLinkedPatientIds, string type)
        {
            var links = type switch
            {
                "Assigned" => allPatients
                    .Where(p => alreadyLinkedPatientIds.Contains(p.Id))
                    .Select(p => new SelectListItem
                    {
                        Text = $"{p.Name} ({p.Email})",
                        Value = p.Id.ToString()
                    }).ToList(),

                "Available" => allPatients
                    .Where(p => !alreadyLinkedPatientIds.Contains(p.Id))
                    .Select(p => new SelectListItem
                    {
                        Text = $"{p.Name} ({p.Email})",
                        Value = p.Id.ToString()
                    }).ToList(),

                _ => new List<SelectListItem>(),
            };
            return Task.FromResult<IList<SelectListItem>>(links);
        }
    }
}