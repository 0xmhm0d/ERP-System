using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ERP_BusinessLogic.Helpers;
using ERP_BusinessLogic.Services;
using ERP_Domians.IServices;
using ERP_Domians.Models;
using GP_ERP_SYSTEM_v1._0.DTOs;
using GP_ERP_SYSTEM_v1._0.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAuthService authService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO registerUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var user = new ApplicationUser()
                {
                    Email = registerUserDTO.Email,
                    UserName = registerUserDTO.Email,
                    FirstName = registerUserDTO.FirstName,
                    LastName = registerUserDTO.LastName
                };

                var result = await _authService.RegisterAsync(user, registerUserDTO.Password);

                if (result.IsAuthenticated == false)
                    return BadRequest(result.Message);

                return Ok(new UserDTO
                {
                    UserName = result.UserName,
                    Email = result.Email,
                    Roles = result.Roles,
                    Token = result.Token,
                    ExpiresOn = result.ExpiresOn,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var result = await _authService.LoginAsync(loginUserDTO.Email, loginUserDTO.Password);

                if (result.IsAuthenticated == false)
                    return BadRequest(result.Message);

                return Ok(new UserDTO
                {
                    UserName = result.UserName,
                    Email = result.Email,
                    Roles = result.Roles,
                    Token = result.Token,
                    ExpiresOn = result.ExpiresOn,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<RoleModel>> AddRoleAsync([FromBody] RoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            var result = await _authService.AddRoleAsync(user.Id, model.Role);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }


        #region Old Code
        //[HttpPost]
        //public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO registerUserDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    try
        //    {
        //        var user = new ApplicationUser()
        //        {
        //            Email = registerUserDTO.Email,
        //            UserName = registerUserDTO.Email,
        //            FirstName = registerUserDTO.FirstName,
        //            LastName = registerUserDTO.LasttName
        //        };

        //        var result = await _userManager.CreateAsync(user, registerUserDTO.Password);

        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, Roles.UserRole);

        //            return new UserDTO()
        //            {
        //                Email = user.Email,
        //                Name = user.FirstName,
        //                Token = _authService.CreateToken(user)
        //            };
        //        }
        //        else
        //            return BadRequest();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
        //    }
        //}


        //[HttpPost]
        //public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginUserDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    try
        //    {
        //        var user = await _userManager.FindByEmailAsync(loginUserDTO.Email);

        //        if (user == null)
        //            return BadRequest(new ErrorValidationResponse() { Errors = new List<string> { "Email doesn't exist." } });

        //        var result = await _userManager.CheckPasswordAsync(user, loginUserDTO.Password);

        //        if (!result)
        //            return BadRequest(new ErrorValidationResponse() { Errors = new List<string> { "Password doesn't match the existing email." } });

        //        return new UserDTO()
        //        {
        //            Email = loginUserDTO.Email,
        //            Name = user.FirstName,
        //            Token = _authService.CreateToken(user)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
        //    }
        //}


        //[HttpGet]
        //[Authorize]
        //public async Task<ActionResult<UserDTO>> GetCurrentUser() {


        //  try {   
        //    var email =  User.FindFirstValue(ClaimTypes.Email);
        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //        return BadRequest();

        //    return Ok(new UserDTO()
        //    {
        //        Email = user.Email,
        //        Name = user.FirstName,
        //        Token = _tokenService.CreateToken(user)
        //    });     
        //  } catch (Exception ex)
        //    {
        //        return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
        //    }

        //}


        //[HttpGet]
        //public async Task<ActionResult<bool>> CheckIfEmailExistsAsync ([FromQuery] string email)
        //{
        //    try {

        //        return await _userManager.FindByEmailAsync(email) != null;

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
        //    }

        //} 
        #endregion

    }
}
