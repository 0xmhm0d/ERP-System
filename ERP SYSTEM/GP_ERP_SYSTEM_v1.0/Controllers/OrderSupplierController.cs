using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.IServices;
using ERP_Domians.Models.HelpersParameters;
using GP_ERP_SYSTEM_v1._0.DTOs;
using GP_ERP_SYSTEM_v1._0.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,SCM")]
    public class OrderSupplierController : ControllerBase
    {
        private readonly ISupplierOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
      //  private readonly Uri baseAddress = new("http://erpv2.runasp.net/api");
        private readonly Uri baseAddress = new("https://localhost:5001/api");

        public OrderSupplierController(ISupplierOrderService orderService, IUnitOfWork unitOfWork, IMapper mapper, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderService = orderService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseAddress;
        }

        #region Supplier Orders

        [HttpGet]
        public async Task<ActionResult> GetAllSupplierOrders()
        {
            try
            {
                var orders = await _unitOfWork.OrderSupplier.GetAllAsync(
                    new List<string>() { "OrderStatus", "OrderedMaterials", "Supplier" },
                    o => o.OrderByDescending(o => o.OrderingDate)
                );

                return orders == null
                    ? NotFound(new ErrorApiResponse(404))
                    : Ok(_mapper.Map<List<OrderSupplierDTO>>(orders));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetSupplierOrderById(int orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderSupplier.FindAsync(o => o.Id == orderId,
                    new List<string>() { "OrderStatus", "OrderedMaterials", "Supplier" }
                );

                return order == null ? NotFound(new ErrorApiResponse(404)) : Ok(_mapper.Map<OrderSupplierDTO>(order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> OrderRawMaterialFromSupplier(int supplierId, decimal shippingCost, List<MaterialsOrderedParameters> orderMaterialParams)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (supplierId <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            if (shippingCost <= 0)
                return BadRequest(new ErrorApiResponse(400, "Shipping Cost can't be 0 or less."));

            if (orderMaterialParams is null)
                return BadRequest(new ErrorApiResponse(400, "Materials Ordered Cannot be null."));

            try
            {
                var supplierMaterialDetails = await
                    _unitOfWork.SupplingMaterialDetails.FindRangeAsync(
                        supplyingDetails => supplyingDetails.SupplierId == supplierId &&
                        orderMaterialParams.Select(materialOrdered => materialOrdered.MaterialId).Contains(supplyingDetails.MaterialId)
                    );

                if (!supplierMaterialDetails.Any())
                    return NotFound(new ErrorApiResponse(404, "The supplier Id associated with the materials ids are not found."));

                if (supplierMaterialDetails.Count() != orderMaterialParams.Count)
                    return BadRequest(new ErrorApiResponse(400, "Some materials Ids are not being provided by this supplier."));

                var result = await _orderService.CreateSupplierOrder(supplierId, shippingCost, orderMaterialParams);

                if (result == null)
                    return StatusCode(500, new ErrorApiResponse(500) { Message = "Error Occurred While Creating your Order" });

                /*Create JE*/
                var supplierJE = new AddFmsJeDTO()
                {
                    Jename = $"Supplier Order from Supplier ID = {result.SupplierId}",
                    Jedescription = $"An Order from Supplier whose ID is {result.SupplierId} with total cash of  $ {result.TotalPrice} and total quantity of {result.TotalQty}",
                    Jeaccount1 = 2,
                    Jeaccount2 = 1,
                    Jecredit = result.TotalPrice,
                    Jedebit = result.TotalPrice,
                    Jedate = result.OrderingDate,
                };

                var shippingCostJE = new AddFmsJeDTO()
                {
                    Jename = $"Shipping Cost of Supplier Order from Supplier ID = {result.SupplierId}",
                    Jedescription = "Cost of Shipping Order from Supplier To the Inventory.",
                    Jeaccount1 = 3,
                    Jeaccount2 = 1,
                    Jecredit = result.ShippingCost,
                    Jedebit = result.ShippingCost,
                    Jedate = result.OrderingDate,
                };

                // Get the token from the Authorization header
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized("No token provided");

                // Create HttpClient and add the token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var createJE = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/AddNewFmsJournalEntry",
                   new StringContent(JsonConvert.SerializeObject(supplierJE), Encoding.UTF8, "application/json"));

                var createJE2 = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/AddNewFmsJournalEntry",
                new StringContent(JsonConvert.SerializeObject(shippingCostJE), Encoding.UTF8, "application/json"));

                // Check if the response was successful
                if (!createJE.IsSuccessStatusCode && !createJE2.IsSuccessStatusCode)
                    return BadRequest();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }
        }

        #endregion

        #region Updating Status APIs

        [HttpPut]
        public async Task<ActionResult> ChangeSupplierOrderStatusToShipped(int orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderSupplier.GetByIdAsync(orderId);

                if (order == null)
                    return NotFound(new ErrorApiResponse(404, "Supplier Order Id is not found."));

                if (order.OrderStatusId != 1)
                    return BadRequest(new ErrorApiResponse(400, "Supplier Order status has to be pending InOrder to change it to shipped.."));

                order.OrderStatusId = 2;
                _unitOfWork.OrderSupplier.Update(order);

                await _unitOfWork.Save();
                return Ok("Status updated from pending to shipped");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> ChangeSupplierOrderStatusToFulfilled(int orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderSupplier.FindAsync(i => i.Id == orderId, new List<string>() { "OrderedMaterials" });

                if (order == null)
                    return NotFound(new ErrorApiResponse(404, "Supplier Order Id is not found."));

                if (order.OrderStatusId != 2)
                    return BadRequest(new ErrorApiResponse(400, "Supplier Order status has to be shipped InOrder to change it to fulfilled."));

                order.OrderStatusId = 3;
                _unitOfWork.OrderSupplier.Update(order);

                foreach (var rawMaterial in order.OrderedMaterials)
                    await UpdateRawMaterialsInventory(rawMaterial.OrderedRawMaterials.MaterialId, rawMaterial.Quantity);

                await _unitOfWork.Save();

                return Ok("status updated from shipped to fulfilled");

            }
            catch (Exception ex)
            {
                //throw;
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }

        }

        [HttpPut]
        public async Task<ActionResult> ChangeSupplierOrderStatusToFailed(int orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderSupplier.GetByIdAsync(orderId);

                if (order == null)
                    return NotFound(new ErrorApiResponse(404, "Supplier Order Id is not found."));

                order.OrderStatusId = 4;
                _unitOfWork.OrderSupplier.Update(order);

                await _unitOfWork.Save();
                return Ok("Status set to failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }
        }

        #endregion

        private async Task UpdateRawMaterialsInventory(int rawMaterialId, int qtyToAdd)
        {
            var rawMaterial = await _unitOfWork.RawMaterialInventory.GetByIdAsync(rawMaterialId);

            rawMaterial.Quantity += qtyToAdd;

            rawMaterial.HasReachedROP = rawMaterial.Quantity <= rawMaterial.ReorderingPoint;

            _unitOfWork.RawMaterialInventory.Update(rawMaterial);
        }
    }
}
