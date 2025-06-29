using ERP_BusinessLogic.Context;
using ERP_BusinessLogic.Helpers;
using ERP_Domians.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP_BusinessLogic.DbInitialize
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ILogger<DbInitializer> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            ApplicationDbContext dbContext,
            ILogger<DbInitializer> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            #region Update Database (Migrations)
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                    await _dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error Occurred During Appling The Migration");
            }
            #endregion

            #region Create Roles Admin User

            if (!await _roleManager.RoleExistsAsync(Roles.AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(Roles.UserRole));
                await _roleManager.CreateAsync(new IdentityRole(Roles.CRMRole));
                await _roleManager.CreateAsync(new IdentityRole(Roles.HRRole));
                await _roleManager.CreateAsync(new IdentityRole(Roles.SCMRole));
                await _roleManager.CreateAsync(new IdentityRole(Roles.FMSRole));
                await _roleManager.CreateAsync(new IdentityRole(Roles.IMMRole));

                var newAdmin = new ApplicationUser
                {
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    FirstName = "Administrator",
                    LastName = "User",
                    PhoneNumber = "01040313631",
                };

                await _userManager.CreateAsync(newAdmin, "P@$$w0rd1234");

                var administrator = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(a => a.FirstName == newAdmin.FirstName);

                await _userManager.AddToRoleAsync(administrator, Roles.AdminRole);
            }

            #endregion

            #region Distribution Order Status

            if (!_dbContext.TbDistributionOrderStatus.Any())
            {
                var distributionOrderStatus = new List<TbDistributionOrderStatus>
                {
                    new(){ Status = "Pending" },
                    new(){ Status = "Shipped" },
                    new(){ Status = "Fulfilled" },
                    new(){ Status = "Delivered" },
                    new(){ Status = "Cancelled" }
                };

                foreach (var status in distributionOrderStatus)
                {
                    await _dbContext.Set<TbDistributionOrderStatus>().AddAsync(status);
                    await _dbContext.SaveChangesAsync();
                }

                //await _dbContext.SaveChangesAsync();
            }

            #endregion

            #region Manufacturing Status

            if (!_dbContext.TbManufacturingStatus.Any())
            {
                var manufacturingStatus = new List<TbManufacturingStatus>
                {
                    new() { statusName = "Pending" },
                    new() { statusName = "Manufacturing" },
                    new() { statusName = "Shipped To Inventory" },
                    new() { statusName = "Cancelled" }
                };

                foreach (var status in manufacturingStatus)
                {
                    await _dbContext.Set<TbManufacturingStatus>().AddAsync(status);
                    await _dbContext.SaveChangesAsync();
                }

                //await _dbContext.SaveChangesAsync();
            }

            #endregion

            #region Order Suppliers Status

            if (!_dbContext.TbOrderStatus_Suppliers.Any())
            {
                var orderSuppliersStatus = new List<TbOrderStatus_Supplier>
                {
                    new(){OrderStatusName = "Pending"},
                    new(){OrderStatusName = "Shipped"},
                    new(){OrderStatusName = "Fulfilled"},
                    new(){OrderStatusName = "Failed"}
                };

                foreach (var status in orderSuppliersStatus)
                { 
                    await _dbContext.Set<TbOrderStatus_Supplier>().AddAsync(status);
                    await _dbContext.SaveChangesAsync();
                }

                //await _dbContext.SaveChangesAsync();
            }

            #endregion

            #region FMS Accounts

            if (!_dbContext.TbFmsAccounts.Any())
            {
                var fmsAccounts = new List<TbFmsAccount>
                {
                    new() { AccName = "Cash", IncreaseMode = 1 },
                    new() { AccName = "Investment", IncreaseMode = 1 },
                    new() { AccName = "Retirement Account", IncreaseMode = 1 },
                    new() { AccName = "Real Estate", IncreaseMode = 1 },
                    new() { AccName = "Accounts Receivable", IncreaseMode = 1 }
                };
                foreach (var account in fmsAccounts)
                {
                    await _dbContext.Set<TbFmsAccount>().AddAsync(account);
                    await _dbContext.SaveChangesAsync();
                }

                //await _dbContext.SaveChangesAsync();
            }

            #endregion
        }
    }
}