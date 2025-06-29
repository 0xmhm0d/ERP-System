using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.Models;
using GP_ERP_SYSTEM_v1._0.DTOs;
using GP_ERP_SYSTEM_v1._0.Errors;
using Microsoft.AspNetCore.Authorization;


namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]

    [Authorize(Roles = "Admin,FMS")]
    public class FmsAccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FmsAccountsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> FmsGetAllAccounts()
        {
            try
            {
                var accounts = await _unitOfWork.FmsAccount.GetAllAsync();

                return Ok(_mapper.Map<List<FmsAccountDTO>>(accounts));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FmsGetAccountById(int id)
        {
            try
            {
                var account = await _unitOfWork.FmsAccount.GetByIdAsync(id);

                if (account == null)
                    return NotFound(new ErrorApiResponse(404, "Account id is not found"));

                var accountCategory = await _unitOfWork.FmsAccCat.FindRangeAsync(o => o.AccId == id);

                var categories = await _unitOfWork.FmsCategory.GetAllAsync();

                var accountCategoryNames = (from p in accountCategory
                                            join e in categories
                                            on p.CatId equals e.CatId
                                            select e.CatName)
                                            .ToList();

                var result = new ViewFmsAccountDTO
                {
                    AccBalance = account.AccBalance,
                    AccCategories = accountCategoryNames,
                    AccCredit = account.AccCredit,
                    AccDebit = account.AccDebit,
                    AccId = account.AccId,
                    AccName = account.AccName,
                    IncreaseMode = account.IncreaseMode == 0 ? "Debit" : "Credit"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> FmsAddAccount([FromBody] AddFmsAccountDTO accountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var fmsAccount = _mapper.Map<TbFmsAccount>(accountDto);

                _unitOfWork.FmsAccount.InsertAsync(fmsAccount);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> FmsUpdateAccount(int id, [FromBody] AddFmsAccountDTO accountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var accountToUpdate = await _unitOfWork.FmsAccount.GetByIdAsync(id);

                if (accountToUpdate == null)
                    return BadRequest(new ErrorApiResponse(400, "Submitted Account ID is invalid."));

                _mapper.Map(accountDto, accountToUpdate);

                _unitOfWork.FmsAccount.Update(accountToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> FmsDeleteAccount(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var fmsAccountToDelete = await _unitOfWork.FmsAccount.GetByIdAsync(id);

                if (fmsAccountToDelete == null)
                    return BadRequest("Invalid Id is submitted.");

                _unitOfWork.FmsAccount.Delete(fmsAccountToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }
    }
}