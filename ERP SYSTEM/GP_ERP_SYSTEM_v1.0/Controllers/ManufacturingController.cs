using System;
using System.Collections.Generic;
using System.Linq;
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

namespace GP_ERP_SYSTEM_v1._0.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,SCM")]
    public class ManufacturingController : ControllerBase
    {
        private readonly IManufacturingOrderService _manufacturingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ManufacturingController(IManufacturingOrderService manufacturingService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _manufacturingService = manufacturingService;
        }

        #region Manufacturing Order

        [HttpGet]
        public async Task<ActionResult<List<ReturnedManufacturingOrderDTO>>> GetAllManufacturingOrders()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var manufacturingOrders = await _unitOfWork.Manufacturing.GetAllManufacturingOrders();

                return Ok(_mapper.Map<List<ReturnedManufacturingOrderDTO>>(manufacturingOrders));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500, ex.Message));
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReturnedManufacturingOrderDTO>> GetManufacturingOrderById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Enter a valid Manufacturing Id."));

            try
            {
                var manufacturingOrder = await _unitOfWork.Manufacturing.GetManufacturingOrderById(id);

                if (manufacturingOrder == null)
                    return NotFound(new ErrorApiResponse(404, "Manufacturing Id is not found."));

                return Ok(_mapper.Map<ReturnedManufacturingOrderDTO>(manufacturingOrder));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500, ex.Message));
            }

        }

        [HttpPost]
        public async Task<ActionResult> CreateManufacturingOrder(CreateManufacturingOrderDTO manufacturingOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _unitOfWork.Product.GetByIdAsync(manufacturingOrderDto.ProductManufacturedId);

                if (product == null)
                    return BadRequest(new ErrorApiResponse(400, "Enter a valid product ID and try again."));

                var invalidRawMaterials = new List<int>();

                foreach (var rawMaterial in manufacturingOrderDto.RawMaterialsUsed)
                {
                    var inventoryMaterial = await _unitOfWork.RawMaterialInventory.GetByIdAsync(rawMaterial.MaterialId);

                    if (inventoryMaterial == null)
                        invalidRawMaterials.Add(rawMaterial.MaterialId);
                }

                if (invalidRawMaterials.Count > 0)
                    return BadRequest(new ErrorApiResponse(400, $"The following RawMaterial ids are not included in the Inventory : {string.Join(", ", invalidRawMaterials)}"));

                var result = await _manufacturingService
                    .CreateManufacturingOrder(
                    manufacturingOrderDto.ProductManufacturedId,
                    manufacturingOrderDto.QtyToManufacture,
                    manufacturingOrderDto.ManufacturingCost,
                    manufacturingOrderDto.StartingDate,
                    manufacturingOrderDto.RawMaterialsUsed
                    );

                if (result == null)
                    return StatusCode(500, "Error happened while adding order");

                return Ok(_mapper.Map<ReturnedManufacturingOrderDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        } 

        #endregion


        #region Updating Status APIs

        [HttpPut]
        public async Task<ActionResult> ChangeManufacturingStatusToManufacturing(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Manufacturing.GetManufacturingOrderById(orderId);

                if (order == null)
                    return NotFound(new ErrorApiResponse(404, "Manufacturing Order Id is not found."));

                if (order.ManufacturingStatusId != 1)
                    return BadRequest(new ErrorApiResponse(400, "Manufacturing Order status has to be pending InOrder to change it to Manufacturing.."));

                order.ManufacturingStatusId = 2;
                _unitOfWork.Manufacturing.Update(order);

                //Check Raw Materials used if reached ReorderingPoint. if reached ? send/ notify user to order it from the suppliers 
                foreach (var rawMaterial in order.ManufacturingOrderDetails)
                    await UpdateRawMaterialsInventory(rawMaterial.RawMaterialId, rawMaterial.RawMaterialQtyUsed);

                await _unitOfWork.Save();

                return Ok("Status updated from pending to manufacturing");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> ChangeManufacturingStatusToShippedToInventory(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Manufacturing.GetManufacturingOrderById(orderId);

                if (order == null)
                    return NotFound(new ErrorApiResponse(404, "Manufacturing Order Id is not found."));

                if (order.ManufacturingStatusId != 2)
                    return BadRequest(new ErrorApiResponse(400, "Manufacturing Order status has to be manufacturing In Order to change it to ShippedToInventory."));

                order.ManufacturingStatusId = 3;
                _unitOfWork.Manufacturing.Update(order);

                //Update Products Inventory ++QTY + UpdateHasReachedROP
                await UpdateProductsInventory(order.ProductManufacturedId, order.QtyToManufacture);

                await _unitOfWork.Save();

                return Ok("Status updated from manufacturing to shippedToInventory");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorApiResponse(500) { Message = ex.Message });
            }

        }

        #endregion


        #region private helper methods

        private async Task UpdateRawMaterialsInventory(int rawMaterialId, int qtyToRemove)
        {
            var rawMaterial = await _unitOfWork.RawMaterialInventory.GetByIdAsync(rawMaterialId);

            rawMaterial.Quantity -= qtyToRemove;

            rawMaterial.HasReachedROP = rawMaterial.Quantity <= rawMaterial.ReorderingPoint ? true : false;

            //if (rawMaterial.Quantity <= rawMaterial.ReorderingPoint)
            //    rawMaterial.HasReachedROP = true;
            //else
            //    rawMaterial.HasReachedROP = false;

            _unitOfWork.RawMaterialInventory.Update(rawMaterial);
        }

        private async Task UpdateProductsInventory(int productId, int qtyToAdd)
        {
            var product = await _unitOfWork.ProductsInventory.GetByIdAsync(productId);

            product.Quantity += qtyToAdd;

            product.HasReachedROP = product.Quantity > product.ReorderingPoint ? false : true;

            //if (product.Quantity > product.ReorderingPoint)
            //    product.HasReachedROP = false;
            //else
            //    product.HasReachedROP = true;

            _unitOfWork.ProductsInventory.Update(product);

            await _unitOfWork.Save();
        }

        #endregion
    }
}
