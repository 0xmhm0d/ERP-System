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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,SCM")]
    public class RawMaterialController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RawMaterialController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRawMaterials()
        {
            try
            {
                var RawMaterials = await _unitOfWork.RawMaterial.GetAllAsync();

                return Ok(_mapper.Map<List<RawMaterialDTO>>(RawMaterials));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRawMaterialById(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var rawMaterial = await _unitOfWork.RawMaterial.GetByIdAsync(id);

                if (rawMaterial == null)
                    return NotFound(new ErrorApiResponse(404, "Raw Material is not found."));

                return Ok(_mapper.Map<RawMaterialDTO>(rawMaterial));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewRawMaterial([FromBody] AddRawMaterialDTO rawMaterialDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _unitOfWork.RawMaterial.InsertAsync(_mapper.Map<TbRawMaterial>(rawMaterialDto));
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRawMaterial(int id, [FromBody] AddRawMaterialDTO rawMaterial)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorValidationResponse() { Errors = new List<string> { "Id can't be 0 or less." } });
            try
            {
                var rawMaterialToUpdate = await _unitOfWork.RawMaterial.GetByIdAsync(id);

                if (rawMaterialToUpdate == null)
                    return NotFound(new ErrorApiResponse(404, "RawMaterial is not found."));

                _mapper.Map(rawMaterial, rawMaterialToUpdate);

                _unitOfWork.RawMaterial.Update(rawMaterialToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRawMaterial(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var rawMaterialToDelete = await _unitOfWork.RawMaterial.GetByIdAsync(id);

                if (rawMaterialToDelete == null)
                    return NotFound(new ErrorApiResponse(404, "Raw Material is not found."));

                _unitOfWork.RawMaterial.Delete(rawMaterialToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }
    }
}
