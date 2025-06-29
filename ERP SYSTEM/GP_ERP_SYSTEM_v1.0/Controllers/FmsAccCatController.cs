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
    public class FmsAccCatController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FmsAccCatController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> FmsGetAllAccCats()
        {
            try
            {
                var accCats = await _unitOfWork.FmsAccCat.GetAllAsync();

                return Ok(_mapper.Map<List<FmsAccCatDTO>>(accCats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> FmsGetCategoryAccounts(int catID)
        {
            if (catID <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var catAccts = await _unitOfWork.FmsAccCat.FindRangeAsync(o => o.CatId == catID);

                return Ok(_mapper.Map<List<FmsAccCatDTO>>(catAccts));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> FmsGetAccountCategories(int accID)
        {
            if (accID <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var accCats = await _unitOfWork.FmsAccCat.FindRangeAsync(o => o.AccId == accID);

                return Ok(_mapper.Map<List<FmsAccCatDTO>>(accCats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> FmsAddAccCat([FromBody] FmsAccCatDTO accountCategory)
        {
            var category = await _unitOfWork.FmsCategory.GetByIdAsync(accountCategory.CatId);

            if (category == null)
                return NotFound(new ErrorApiResponse(404, "Category id is not found"));

            var account = await _unitOfWork.FmsAccount.GetByIdAsync(accountCategory.AccId);

            if (account == null)
                return NotFound(new ErrorApiResponse(404, "Account id is not found"));

            try
            {
                _unitOfWork.FmsAccCat.InsertAsync(_mapper.Map<TbFmsAccCat>(accountCategory));

                await _unitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete]
        public async Task<IActionResult> FmsDeleteAccCat(int accID, int catID)
        {
            if (catID <= 0 || accID <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var fmsAccCatToDelete = await _unitOfWork.FmsAccCat.FindAsync(o => o.AccId == accID && o.CatId == catID);

                if (fmsAccCatToDelete == null)
                    return BadRequest("Invalid Id is submitted.");

                _unitOfWork.FmsAccCat.Delete(fmsAccCatToDelete);
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
