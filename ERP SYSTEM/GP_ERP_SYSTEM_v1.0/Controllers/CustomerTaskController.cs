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
    [Authorize(Roles = "Admin,CRM")]
    public class CustomerTaskController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerTaskController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomerTasks()
        {
            try
            {
                var customersTask = await _unitOfWork.Task.GetAllAsync(new List<string>() { "Customer" });

                return Ok(_mapper.Map<List<TaskDTO>>(customersTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error." + ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerTasksById(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorValidationResponse() { Errors = new List<string> { "Id can't be 0 or less." } });

            try
            {
                var CustomerTaskId = await _unitOfWork.Task.FindAsync(P => P.TaskId == id, new List<string>() { "Customer" });
                if (CustomerTaskId == null)
                    return BadRequest(new ErrorApiResponse(404, "   Customer's Task Not Found."));

                return Ok(_mapper.Map<TaskDTO>(CustomerTaskId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddNewTaskForCustomer([FromBody] AddTaskDTO customerTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!ValidateCustomerId(customerTaskDto.CustomerId))
                return BadRequest(new ErrorApiResponse(400, "Invalid Customer's is sent."));

            try
            {
                _unitOfWork.Task.InsertAsync(_mapper.Map<TbTask>(customerTaskDto));
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerTask(int id, [FromBody] AddTaskDTO customerTaskDto)
        {
            if (id <= 0)
                return BadRequest(new ErrorValidationResponse() { Errors = new List<string> { "Id can't be 0 or less." } });


            if (!ValidateCustomerId(customerTaskDto.CustomerId))
                return BadRequest(new ErrorApiResponse(400, "Invalid Customer's is sent."));

            try
            {
                var customerTaskToUpdate = await _unitOfWork.Task.GetByIdAsync(id);

                if (customerTaskToUpdate == null)
                    return BadRequest("Invalid Customer's Task Id Is Submitted");

                _mapper.Map(customerTaskDto, customerTaskToUpdate);

                _unitOfWork.Task.Update(customerTaskToUpdate);

                await _unitOfWork.Save();


                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCustomerTaskById(int id)
        {
            if (id < 1)
                return BadRequest("ID can't be 0 or less");

            try
            {
                var CustomerTaskIdToDelete = await _unitOfWork.Task.GetByIdAsync(id);

                if (CustomerTaskIdToDelete == null)
                    return BadRequest("Invalid Custoemr's Id Is Submitted");

                _unitOfWork.Task.Delete(CustomerTaskIdToDelete);

                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }


        private bool ValidateCustomerId(int customerId)
        {
            var customerIdsList = _unitOfWork.Customer.GetAllAsync().Result?.Select(e => e.CustomerId);

            if (customerIdsList == null)
                return false;

            return customerIdsList.Contains(customerId);
        }
    }
}
