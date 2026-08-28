using ITI_Project.Constants;
using Microsoft.AspNetCore.Identity;

namespace ITI_Project.Data
{
    public class DBSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var UserMgr = service.GetService<UserManager<IdentityUser>>();
            var RoleMgr = service.GetService<RoleManager<IdentityRole>>();
            //adding some roles to db >> 1-Admin , 2-User ,,,,AspNetRoles بيتحطوا في جدول 

            if (!await RoleMgr.RoleExistsAsync(Roles.Admin.ToString()))
                await RoleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));

            if (!await RoleMgr.RoleExistsAsync(Roles.User.ToString()))
                await RoleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));


            //Create Admin User
            var admin = new IdentityUser
            {
                UserName = "aeisa7123@gmail.com",
                Email = "aeisa7123@gmail.com",
                EmailConfirmed = true
            };

            var UserInDb = await UserMgr.FindByEmailAsync(admin.Email);
            if (UserInDb is null)
            {
                await UserMgr.CreateAsync(admin, "Admin@123");
                await UserMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }







        }
    }
}
