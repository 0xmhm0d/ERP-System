using AutoMapper;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.Models;
using GP_ERP_SYSTEM_v1._0.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using GP_ERP_SYSTEM_v1._0.Errors;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    public class EmployeeTaskController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeTaskController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployeeTasks()
        {
            try
            {
                var employeesTask = await _unitOfWork.EmployeeTask.GetAllAsync(new List<string>() { "Emplyee" });

                return Ok(_mapper.Map<List<EmployeeTaskDTO>>(employeesTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeTasksById(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var employeeTaskId = await _unitOfWork.EmployeeTask.FindAsync(P => P.TaskId == id, new List<string>() { "Emplyee" });

                if (employeeTaskId == null)
                    return BadRequest(new ErrorApiResponse(404, "Employee's Task Not Found."));

                return Ok(_mapper.Map<EmployeeTaskDTO>(employeeTaskId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTaskForEmployee([FromBody] AddEmployeeTaskDTO taskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!ValidateEmployeeId(taskDto.EmplyeeId))
                    return BadRequest(new ErrorApiResponse(400, "Invalid Employee's is sent."));

                _unitOfWork.EmployeeTask.InsertAsync(_mapper.Map<TbEmployeeTaskDetail>(taskDto));
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeTask(int id, [FromBody] AddEmployeeTaskDTO taskDto)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            if (!ValidateEmployeeId(taskDto.EmplyeeId))
                return BadRequest(new ErrorApiResponse(400, "Invalid Employee's is sent."));

            try
            {
                var taskIdToUpdate = await _unitOfWork.EmployeeTask.GetByIdAsync(id);

                if (taskIdToUpdate == null)
                    return BadRequest("Invalid Employee's Task Id Is Submitted");

                _mapper.Map(taskDto, taskIdToUpdate);

                _unitOfWork.EmployeeTask.Update(taskIdToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeeTaskById(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var employeeTaskIdToDelete = await _unitOfWork.EmployeeTask.GetByIdAsync(id);

                if (employeeTaskIdToDelete == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid Employee's Id Is Submitted"));

                _unitOfWork.EmployeeTask.Delete(employeeTaskIdToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        private bool ValidateEmployeeId(int employeeId)
        {
            var employeeIdsList = _unitOfWork.Employee.GetAllAsync().Result?.Select(e => e.EmployeeId);

            if (employeeIdsList == null)
                return false;

            return employeeIdsList.Contains(employeeId);
        }
    }
}
