using Microsoft.AspNetCore.Identity;

namespace MIL_LIT.ViewModel
{
    public class ChangeRoleViewModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole<int> > AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole<int> >();
            UserRoles = new List<string>();
        }
    }
}
