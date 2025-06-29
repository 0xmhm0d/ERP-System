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
    public class FmsStatementTemplateController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FmsStatementTemplateController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Templates
        [HttpGet]
        public async Task<IActionResult> FmsGetAllTemplates()
        {
            try
            {
                var templates = await _unitOfWork.FmsStatementTemplate.GetAllAsync();

                var templatesDTO = new List<ViewFmsTemplateListDTO>();

                foreach (var template in templates)
                {
                    var templateAccounts = await _unitOfWork.FmsTemplateAccount.FindRangeAsync(o => o.TempId == template.TempId);

                    var templateAccountIds = new List<int>();

                    foreach (TbFmsTemplateAccount TemplateAccount in templateAccounts)
                        templateAccountIds.Add(TemplateAccount.AccId);

                    var viewFmsTemplateDTO = new ViewFmsTemplateListDTO()
                    {
                        Accounts = templateAccountIds,
                        TempId = template.TempId,
                        TempDate = template.TempDate,
                        TempName = template.TempName,
                    };

                    templatesDTO.Add(viewFmsTemplateDTO);
                }
                return Ok(templatesDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FmsGetTemplateById(int id)
        {
            try
            {
                var template = await _unitOfWork.FmsStatementTemplate.GetByIdAsync(id);

                var tempAccountIds =
                    (await _unitOfWork.FmsTemplateAccount.FindRangeAsync(o => o.TempId == id))
                    .Select(p => p.AccId)
                    .ToList();

                var accounts = new List<FmsAccountDTO>();

                foreach (var accountId in tempAccountIds)
                {
                    var account = _mapper.Map<FmsAccountDTO>(await _unitOfWork.FmsAccount.GetByIdAsync(accountId));

                    accounts.Add(account);
                }

                var tempWithAccounts = new ViewFmsTemplateDTO()
                {
                    TempId = id,
                    TempName = template.TempName,
                    TempDate = template.TempDate,
                    Accounts = accounts
                };

                return Ok(tempWithAccounts);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> FmsAddTemplate([FromBody] AddFmsTemplateDTO templateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _unitOfWork.FmsStatementTemplate.InsertAsync(_mapper.Map<TbFmsStatementTemplate>(templateDto));

                await _unitOfWork.Save();

                TbFmsStatementTemplate savedTemplate = await _unitOfWork.FmsStatementTemplate.FindAsync(o => o.TempDate == templateDto.TempDate);

                var tempAccounts = new List<TbFmsTemplateAccount>();

                foreach (var account in templateDto.Accounts)
                    tempAccounts.Add(new TbFmsTemplateAccount { AccId = account.AccId, TempId = savedTemplate.TempId });

                _unitOfWork.FmsTemplateAccount.InsertRangeAsync(tempAccounts);

                await _unitOfWork.Save();
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> FmsUpdateTemplate(int id, [FromBody] AddFmsTemplateDTO templateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var templateToUpdate = await _unitOfWork.FmsStatementTemplate.GetByIdAsync(id);

                if (templateToUpdate == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid Id is submitted."));

                _mapper.Map(templateDto, templateToUpdate);

                _unitOfWork.FmsStatementTemplate.Update(templateToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }

            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete]
        public async Task<IActionResult> FmsDeleteTemplate(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var fmsStatementTemplateToDelete = await _unitOfWork.FmsStatementTemplate.GetByIdAsync(id);

                var fmsTempletAccountsToDelete = (await _unitOfWork.FmsTemplateAccount.FindRangeAsync(p => p.TempId == id)).ToList();

                if (fmsStatementTemplateToDelete == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid Id is submitted."));

                _unitOfWork.FmsTemplateAccount.DeleteRange(fmsTempletAccountsToDelete);
                _unitOfWork.FmsStatementTemplate.Delete(fmsStatementTemplateToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }
        #endregion

        #region TemplateAccount
        [HttpPost]
        public async Task<IActionResult> FmsAddTemplateAccount(int tempID, int accID)
        {
            var templateAccount = new FmsTemplateAccountDTO { TempId = tempID, AccId = accID };

            try
            {
                _unitOfWork.FmsTemplateAccount.InsertAsync(_mapper.Map<TbFmsTemplateAccount>(templateAccount));

                await _unitOfWork.Save();
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> FmsGetTemplateAccounts(int tempID)
        {
            try
            {
                var accounts = await _unitOfWork.FmsTemplateAccount.FindRangeAsync(o => o.TempId == tempID);

                return Ok(_mapper.Map<List<TbFmsTemplateAccount>>(accounts));
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete]
        public async Task<IActionResult> FmsDeleteTemplateAccount(int tempID)
        {
            if (tempID <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var fmsTemplateAccountToDelete = await _unitOfWork.FmsTemplateAccount.FindAsync(o => o.TempId == tempID);

                if (fmsTemplateAccountToDelete == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid Id is submitted."));

                _unitOfWork.FmsTemplateAccount.Delete(fmsTemplateAccountToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }
        #endregion
    }
}
