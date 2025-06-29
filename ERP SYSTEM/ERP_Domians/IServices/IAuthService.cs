using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_Domians.Models;
using ERP_Domians.Models.ReturnModel;

namespace ERP_Domians.IServices
{
    public interface IAuthService
    {
        Task<ReturnedAuthModel> RegisterAsync(ApplicationUser user, string password);
        Task<ReturnedAuthModel> LoginAsync(string email, string password);
        Task<string> AddRoleAsync(string userId, string role);

        //string CreateToken(ApplicationUser applicationUser);
    }
}
