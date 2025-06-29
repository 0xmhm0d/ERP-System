using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domains.Interfaces.IUnitOfWork;
using ERP_BusinessLogic.Context;
using ERP_Domians.Models;
using GP_ERP_SYSTEM_v1._0.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,FMS")]
    public class FmsJEController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public FmsJEController(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllJournals()
        {
            try
            {
                var Journals = await _unitOfWork.FmsJournalEntry.GetAllAsync();
                return Ok(_mapper.Map<List<FmsJeDTO>>(Journals));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFmsJournalEntryById(int id)
        {
            try
            {
                var FmsJournalEntry = await _unitOfWork.FmsJournalEntry.GetByIdAsync(id);
                return Ok(_mapper.Map<FmsJeDTO>(FmsJournalEntry));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddNewFmsJournalEntry([FromBody] AddFmsJeDTO addFmsJournalEntryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Retrieve affected accounts
                TbFmsAccount debitAccount = await _unitOfWork.FmsAccount.GetByIdAsync(addFmsJournalEntryDto.Jeaccount1);
                TbFmsAccount creditAccount = await _unitOfWork.FmsAccount.GetByIdAsync(addFmsJournalEntryDto.Jeaccount2);

                if (debitAccount == null || creditAccount == null)
                    return NotFound("One or both accounts not found");

                // Use transaction for atomic operations
                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // Initialize null values
                    debitAccount.AccDebit ??= 0m;
                    debitAccount.AccCredit ??= 0m;
                    debitAccount.AccBalance ??= 0m;
                    creditAccount.AccDebit ??= 0m;
                    creditAccount.AccCredit ??= 0m;
                    creditAccount.AccBalance ??= 0m;

                    // Update debit account
                    decimal debitAmount = addFmsJournalEntryDto.Jedebit ?? 0m;
                    debitAccount.AccDebit += debitAmount;

                    debitAccount.AccBalance = CalculateNewBalance(
                        debitAccount.AccBalance.Value,
                        debitAmount,
                        debitAccount.IncreaseMode == 0
                    );

                    // Update credit account
                    decimal creditAmount = addFmsJournalEntryDto.Jecredit ?? 0m;
                    creditAccount.AccCredit += creditAmount;

                    creditAccount.AccBalance = CalculateNewBalance(
                        creditAccount.AccBalance.Value,
                        creditAmount,
                        creditAccount.IncreaseMode == 1
                    );

                    // Apply changes
                    _unitOfWork.FmsAccount.Update(debitAccount);
                    _unitOfWork.FmsAccount.Update(creditAccount);

                    _unitOfWork.FmsJournalEntry.InsertAsync(_mapper.Map<TbFmsJournalEntry>(addFmsJournalEntryDto));

                    await _unitOfWork.Save();
                    await transaction.CommitAsync();

                    return NoContent();

                    #region Old Version
                    //////apply changes

                    ////debit Account 
                    //debitAccount.AccDebit += addFmsJournalEntryDto.Jedebit;

                    //if (debitAccount.IncreaseMode == 0)
                    //{
                    //    debitAccount.AccBalance += addFmsJournalEntryDto.Jedebit;
                    //}
                    //else
                    //{
                    //    debitAccount.AccBalance -= addFmsJournalEntryDto.Jedebit;
                    //}

                    ////credit account
                    //creditAccount.AccCredit += addFmsJournalEntryDto.Jecredit;

                    //if (creditAccount.IncreaseMode == 1)
                    //{
                    //    creditAccount.AccBalance += addFmsJournalEntryDto.Jecredit;
                    //}
                    //else
                    //{
                    //    creditAccount.AccBalance -= addFmsJournalEntryDto.Jecredit;
                    //}

                    //_unitOfWork.FmsAccount.Update(creditAccount);
                    //_unitOfWork.FmsAccount.Update(debitAccount);

                    //_unitOfWork.FmsJournalEntry.InsertAsync(_mapper.Map<TbFmsJournalEntry>(addFmsJournalEntryDto));

                    //await _unitOfWork.Save();

                    //return NoContent();
                    #endregion
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFmsJournalEntry(int id, [FromBody] AddFmsJeDTO fmsJournalEntryDto)
        {
            if (!ModelState.IsValid || id < 1)
                return BadRequest(ModelState);

            try
            {
                var fmsJournalEntryToUpdate = await _unitOfWork.FmsJournalEntry.GetByIdAsync(id);

                if (fmsJournalEntryToUpdate == null)
                    return BadRequest("submitted FmsJournalEntryId is invalid.");

                _mapper.Map(fmsJournalEntryDto, fmsJournalEntryToUpdate);

                _unitOfWork.FmsJournalEntry.Update(fmsJournalEntryToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteFmsJournalEntry(int id)
        {
            if (id < 1)
                return BadRequest("Id cannot be 0 or less.");

            try
            {
                var fmsJournalEntryToDelete = await _unitOfWork.FmsJournalEntry.GetByIdAsync(id);

                if (fmsJournalEntryToDelete == null)
                    return BadRequest("Invalid Id is submitted.");

                _unitOfWork.FmsJournalEntry.Delete(fmsJournalEntryToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }


        // Helper method for balance calculation
        private static decimal CalculateNewBalance(decimal currentBalance, decimal amount, bool isIncrease)
        {
            return isIncrease ? currentBalance + amount : currentBalance - amount;
        }
    }
}
