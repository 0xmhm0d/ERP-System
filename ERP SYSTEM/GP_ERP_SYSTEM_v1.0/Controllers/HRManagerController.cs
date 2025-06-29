using AutoMapper;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.Models;
using GP_ERP_SYSTEM_v1._0.DTOs;
using GP_ERP_SYSTEM_v1._0.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    public class HRManagerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HRManagerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHRMangers()
        {
            try
            {
                var hrManagers = await _unitOfWork.Hrmanager.GetAllAsync();

                return Ok(_mapper.Map<List<HRManagerDTO>>(hrManagers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHRManagerByID(int id)
        {
            try
            {
                var hrManagerId = await _unitOfWork.Hrmanager.GetByIdAsync(id);

                if (hrManagerId == null)
                    return NotFound(new ErrorApiResponse(404, "HR Manager Id is not found."));

                return Ok(_mapper.Map<AddHRManagerDTO>(hrManagerId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewHRManager([FromBody] AddHRManagerDTO hrManagerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _unitOfWork.Hrmanager.InsertAsync(_mapper.Map<TbHrmanagerDetail>(hrManagerDto));

                await _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHRManager(int id, [FromBody] AddHRManagerDTO hrManagerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var hrManagerIdToUpdate = await _unitOfWork.Hrmanager.GetByIdAsync(id);

                if (hrManagerIdToUpdate == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid HRManager's Id Is Submitted"));

                _mapper.Map(hrManagerDto, hrManagerIdToUpdate);

                _unitOfWork.Hrmanager.Update(hrManagerIdToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteHRManagerById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var hrIdToDelete = await _unitOfWork.Hrmanager.GetByIdAsync(id);

                if (hrIdToDelete == null)
                    return BadRequest("Invalid Employee's Id Is Submitted");

                _unitOfWork.Hrmanager.Delete(hrIdToDelete);

                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }
    }
}
