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
    public class FmsCategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FmsCategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> FmsGetAllCategories()
        {
            try
            {
                var categories = await _unitOfWork.FmsCategory.GetAllAsync();

                return Ok(_mapper.Map<List<FmsCategoryDTO>>(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FmsGetCategoryById(int id)
        {
            try
            {
                var category = await _unitOfWork.FmsCategory.GetByIdAsync(id);

                if (category == null)
                    return NotFound(new ErrorApiResponse(404, "Category id is not found"));

                var accountCategory = await _unitOfWork.FmsAccCat.FindRangeAsync(o => o.CatId == id);

                var accounts = await _unitOfWork.FmsAccount.GetAllAsync();

                var accountCategoriesNames = (from p in accountCategory
                                              join e in accounts
                                              on p.AccId equals e.AccId
                                              select e.AccName)
                                              .ToList();

                var viewFmsCategoryDTO = new ViewFmsCategoryDTO()
                {
                    CatAccounts = accountCategoriesNames,
                    CatDescription = category.CatDescription,
                    CatId = category.CatId,
                    CatName = category.CatName
                };

                return Ok(viewFmsCategoryDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> FmsAddCategory([FromBody] AddFmsCategoryDTO fmsCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _unitOfWork.FmsCategory.InsertAsync(_mapper.Map<TbFmsCategory>(fmsCategoryDto));
                await _unitOfWork.Save();

                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> FmsUpdateCategory(int id, [FromBody] AddFmsCategoryDTO fmsCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var categoryToUpdate = await _unitOfWork.FmsCategory.GetByIdAsync(id);

                if (categoryToUpdate == null)
                    return BadRequest(new ErrorApiResponse(400, "Submitted ID is Invalid."));


                _mapper.Map(fmsCategoryDto, categoryToUpdate);

                _unitOfWork.FmsCategory.Update(categoryToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }

            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete]
        public async Task<IActionResult> FmsDeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var fmsCategoryToDelete = await _unitOfWork.FmsCategory.GetByIdAsync(id);

                if (fmsCategoryToDelete == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid Id is submitted."));

                var associatedAccountCategories = (await _unitOfWork.FmsAccCat.FindRangeAsync(p => p.CatId == id)).ToList();

                _unitOfWork.FmsAccCat.DeleteRange(associatedAccountCategories);
                _unitOfWork.FmsCategory.Delete(fmsCategoryToDelete);
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
