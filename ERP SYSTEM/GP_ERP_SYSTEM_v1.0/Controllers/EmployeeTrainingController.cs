using AutoMapper;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.Models;
using GP_ERP_SYSTEM_v1._0.DTOs;
using GP_ERP_SYSTEM_v1._0.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    public class EmployeeTrainingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeTrainingController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllTrainingEmployee()
        {
            try
            {
                var trainings = await _unitOfWork.TrainningEmployee.GetAllAsync(new List<string>() { "Hrmanger", "Employee" });

                return Ok(_mapper.Map<List<EmployeeTrainningDTO>>(trainings));
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetEmployeeTrainingById(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var trainingId = await _unitOfWork.TrainningEmployee
                    .FindAsync(P => P.TrainnningId == id, new List<string>() { "Hrmanger", "Employee" });

                if (trainingId == null)
                    return BadRequest(new ErrorApiResponse(404, "TrainingId Not Found."));

                return Ok(_mapper.Map<EmployeeTrainningDTO>(trainingId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeeTraining([FromBody] AddEmployeeTrainningDTO employeeTrainingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!ValidateHrId(employeeTrainingDto.Hrid))
                    return BadRequest(new ErrorApiResponse(400, "Invalid HR Id Submitted"));

                if (!ValidateEmployeeId(employeeTrainingDto.EmployeeId))
                    return BadRequest(new ErrorApiResponse(400, "Invalid Employee Id Submitted"));

                _unitOfWork.TrainningEmployee.InsertAsync(_mapper.Map<TbEmployeeTrainning>(employeeTrainingDto));

                await _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTraining(int id, [FromBody] AddEmployeeTrainningDTO employeeTrainingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var employeeTrainingToUpdate = await _unitOfWork.TrainningEmployee.GetByIdAsync(id);

                if (employeeTrainingToUpdate == null)
                    return NotFound(new ErrorApiResponse(404, "Invalid Employee Training's Id Is Submitted"));

                _mapper.Map(employeeTrainingDto, employeeTrainingToUpdate);

                _unitOfWork.TrainningEmployee.Update(employeeTrainingToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeeTrainingById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));
            try
            {
                var employeeTrainingIdToDelete = await _unitOfWork.TrainningEmployee.GetByIdAsync(id);

                if (employeeTrainingIdToDelete == null)
                    return NotFound(new ErrorApiResponse(404, "Invalid Training's Id Is Submitted"));

                _unitOfWork.TrainningEmployee.Delete(employeeTrainingIdToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        private bool ValidateHrId(int hrId)
        {
            var hrManagerIdsList = _unitOfWork.Hrmanager.GetAllAsync().Result.Select(Hr => Hr.Hrid);
            return hrManagerIdsList.Contains(hrId);
        }

        private bool ValidateEmployeeId(int empId)
        {
            var employeeIdsList = _unitOfWork.Employee.GetAllAsync().Result.Select(emp => emp.EmployeeId);
            return employeeIdsList.Contains(empId);
        }
    }
}
