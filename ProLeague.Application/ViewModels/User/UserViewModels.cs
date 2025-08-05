// ProLeague.Application/ViewModels/User/UserViewModels.cs
namespace ProLeague.Application.ViewModels.User
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public List<string> AllRoles { get; set; } = new();
        public IList<string> UserRoles { get; set; } = new List<string>();
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}