﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.IServices;
using ERP_Domians.Models;
using ERP_Domians.Models.HelpersParameters;
using GP_ERP_SYSTEM_v1._0.DTOs;
using GP_ERP_SYSTEM_v1._0.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,SCM")]

    public class DistributorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributionOrderService _distributionOrderService;
        private readonly HttpClient _httpClient;
      //  private readonly Uri baseAddress = new("http://erpv2.runasp.net/api");
        private readonly Uri baseAddress = new("https://localhost:5001/api");

        public DistributorController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IDistributionOrderService distributionOrderService,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributionOrderService = distributionOrderService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseAddress;
        }

        #region Distributor
        [HttpGet]
        public async Task<IActionResult> GetAllDistributors()
        {
            try
            {
                var distributors = await _unitOfWork.Distributor.GetAllAsync();
                return Ok(_mapper.Map<List<DistributorDTO>>(distributors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistributorById(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorValidationResponse() { Errors = new List<string> { "Id can't be 0 or less." } });

            try
            {
                var distributor = await _unitOfWork.Distributor.GetByIdAsync(id);

                if (distributor == null)
                    return NotFound(new ErrorApiResponse(400));

                return Ok(_mapper.Map<DistributorDTO>(distributor));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddNewDistributor([FromBody] AddDistributorDTO distributorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _unitOfWork.Distributor.InsertAsync(_mapper.Map<TbDistributor>(distributorDTO));
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistributor(int id, [FromBody] AddDistributorDTO updateDistributorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorValidationResponse() { Errors = new List<string> { "Id can't be 0 or less." } });

            try
            {
                var distributorToUpdate = await _unitOfWork.Distributor.GetByIdAsync(id);

                if (distributorToUpdate == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid Id is sent."));


                _mapper.Map(updateDistributorDto, distributorToUpdate);

                _unitOfWork.Distributor.Update(distributorToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteDistributor(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id cannot be 0 or less."));

            try
            {
                var distributorToDelete = await _unitOfWork.Distributor.GetByIdAsync(id);

                if (distributorToDelete == null)
                    return BadRequest(new ErrorApiResponse(400, "Invalid Id is sent."));

                _unitOfWork.Distributor.Delete(distributorToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }
        #endregion

        #region Distribution Order

        [HttpGet]
        public async Task<ActionResult<List<ReturnedDistributionOrderDTO>>> GetAllDistributionOrders()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var distributionOrders = await _unitOfWork.Distribution.GetAllDistributionOrders();

                return Ok(_mapper.Map<List<ReturnedDistributionOrderDTO>>(distributionOrders));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReturnedDistributionOrderDTO>> GetDistributionOrderById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var distributionOrder = await _unitOfWork.Distribution.GetDistributionOrderById(id);

                if (distributionOrder == null)
                    return NotFound(new ErrorApiResponse(404, "Distribution Order Id is not found."));

                return Ok(_mapper.Map<ReturnedDistributionOrderDTO>(distributionOrder));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateDistributionOrder([FromBody] CreateDistributionOrderDTO createDistributionOrderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var distributor = await _unitOfWork.Distributor.GetByIdAsync(createDistributionOrderDTO.DistributorId);

                if (distributor == null)
                    return NotFound(new ErrorApiResponse(404, "Distributor id is not found"));

                var invalidProductsIds = new List<int>();

                foreach (var finishedProduct in createDistributionOrderDTO.ProductsOrdered)
                {
                    var inventoryFinishedProduct = await _unitOfWork.ProductsInventory.GetByIdAsync(finishedProduct.ProductId);

                    if (inventoryFinishedProduct == null)
                        invalidProductsIds.Add(finishedProduct.ProductId);
                }

                if (invalidProductsIds.Count > 0)
                    return BadRequest(new ErrorApiResponse(400, $"The following Products ids are not included in the inventory : [{string.Join(", ", invalidProductsIds)}]"));

                var order = await _distributionOrderService
                    .CreateDistributionOrder(createDistributionOrderDTO.DistributorId, createDistributionOrderDTO.ProductsOrdered);

                if (order == null)
                    return StatusCode(500, "An unexpected error occurred while creating your order. please try again later");

                /*Create JE*/
                var JE = new AddFmsJeDTO()
                {
                    Jename = "Distribution Order from Distributor ID = " + order.DistributorId,
                    Jedescription = $"Distribution Order from Distributor {order.DistributorId} with total cash of ${order.TotalPrice} and total quantity of {order.TotalQty}",
                    Jeaccount1 = 1,
                    Jeaccount2 = 2,
                    Jecredit = order.TotalPrice,
                    Jedebit = order.TotalPrice,
                    Jedate = order.OrderingDate,
                };

                // Get the token from the Authorization header
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized("No token provided");

                // Create HttpClient and add the token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var createJE = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/AddNewFmsJournalEntry",
                   new StringContent(JsonConvert.SerializeObject(JE), Encoding.UTF8, "application/json"));

                // Check if the response was successful
                if (!createJE.IsSuccessStatusCode)
                    return BadRequest();

                return Ok(_mapper.Map<ReturnedDistributionOrderDTO>(order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
                //throw;
            }
        }

        #endregion

        #region Updating Status Endpoints

        [HttpPut]
        public async Task<ActionResult> ChangeDistributionStatusToShipped(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Distribution.GetByIdAsync(orderId);

                if (order == null)
                    return NotFound(new ErrorApiResponse(404, "Distribution Order Id is not found."));

                if (order.OrderStatusId != 1)
                    return BadRequest(new ErrorApiResponse(400, "Distribution Order status has to be pending InOrder to change it to shipped.."));

                order.OrderStatusId = 2;

                _unitOfWork.Distribution.Update(order);

                await _unitOfWork.Save();

                return Ok("Status updated from pending to shipped");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> ChangeDistributionStatusToFulfilled(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Distribution.GetDistributionOrderById(orderId);

                if (order == null)
                    return NotFound(new ErrorApiResponse(404, "Distribution Order Id is not found."));

                if (order.OrderStatusId != 2)
                    return BadRequest(new ErrorApiResponse(400, "Distribution Order status has to be shipped InOrder to change it to fulfilled.."));

                order.OrderStatusId = 3;

                _unitOfWork.Distribution.Update(order);

                // Remove Qty to distribute + set ROP from the products inventory when its status are shipped.
                foreach (var product in order.DistributionOrderDetails)
                    await UpdateProductsInventory(product.ProductId, product.Qty);

                await _unitOfWork.Save();

                return Ok("Status updated from shipped to fulfilled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500, ex.Message));
            }
        }

        #endregion
 
        private async Task UpdateProductsInventory(int productId, int qtyToRemove)
        {
            var product = await _unitOfWork.ProductsInventory.GetByIdAsync(productId);

            product.Quantity -= qtyToRemove;

            if (product.Quantity <= product.ReorderingPoint)
                product.HasReachedROP = true;

            _unitOfWork.ProductsInventory.Update(product);
        }
    }
}
