// SID: 2408078
namespace GrapheneTrace.Models.Admin
{
    /// <summary>
    /// Model that allows you to send a list of UserViewModels to the AdminHome view model
    /// </summary>
    public class AdminHomeViewModel
    {
        public List<UserViewModel> Users { get; set; } = [];
    }
}
