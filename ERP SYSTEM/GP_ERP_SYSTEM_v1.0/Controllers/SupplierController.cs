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
    public class SupplierController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Supplier

        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            try
            {
                var Suppliers = await _unitOfWork.Supplier.GetAllAsync();

                return Ok(_mapper.Map<List<SupplierDTO>>(Suppliers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsync(id);

                if (supplier == null)
                    return NotFound(new ErrorApiResponse(404, "Supplier is not found"));

                return Ok(_mapper.Map<SupplierDTO>(supplier));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSupplier([FromBody] AddSupplierDTO supplierDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _unitOfWork.Supplier.InsertAsync(_mapper.Map<TbSupplier>(supplierDto));

                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] AddSupplierDTO supplierDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var supplierToUpdate = await _unitOfWork.Supplier.GetByIdAsync(id);

                if (supplierToUpdate == null)
                    return NotFound(new ErrorApiResponse(404, "Supplier Id is not found."));

                _mapper.Map(supplierDto, supplierToUpdate);

                _unitOfWork.Supplier.Update(supplierToUpdate);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var supplierToDelete = await _unitOfWork.Supplier.GetByIdAsync(id);

                if (supplierToDelete == null)
                    return BadRequest(new ErrorApiResponse(400, "Supplier not found."));

                _unitOfWork.Supplier.Delete(supplierToDelete);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        #endregion

        #region Suppling Material Details

        [HttpGet("{supplierId}")]
        public async Task<IActionResult> GetSuppliersMaterials(int supplierId)
        {
            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId);

                if (supplier == null)
                    return BadRequest(new ErrorApiResponse(400, "supplier Id is not found"));

                var supplyingMaterials = await _unitOfWork.SupplingMaterialDetails
                    .FindRangeAsync(m => m.SupplierId == supplierId, new List<string>() { "Material" });

                return Ok(_mapper.Map<List<ReturnedSupplyingMaterialDetailDTO>>(supplyingMaterials));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSupplyingMaterialToSupplier([FromBody] SupplierMaterialDetailsDTO supplyingMaterialDetailsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (supplyingMaterialDetailsDTO.SupplierId <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            if (supplyingMaterialDetailsDTO.SupplyingMaterialDetails is null)
                return BadRequest(new ErrorApiResponse(400, "Supplying material details cannot be null"));

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplyingMaterialDetailsDTO.SupplierId);

                if (supplier == null)
                    return BadRequest(new ErrorApiResponse(400, "Supplier Id is not found"));

                var sentRawMaterialsIds = supplyingMaterialDetailsDTO.SupplyingMaterialDetails.Select(rm => rm.MaterialId).ToList();

                var storedRawMaterialsIdsInDb = (await _unitOfWork.RawMaterial.GetAllAsync()).Select(i => i.MaterialId).ToList();

                var invalidRawMaterialsIds = GetInvalidRawMaterialsIdsSend(sentRawMaterialsIds, storedRawMaterialsIdsInDb);

                if (invalidRawMaterialsIds.Count > 0)
                    return BadRequest(new ErrorApiResponse(400, $"The following Materials ids are invalid: [{string.Join(", ", invalidRawMaterialsIds)}]"));

                var supplyingMaterialsDetails = _mapper.Map<List<TbSupplyingMaterialDetail>>(supplyingMaterialDetailsDTO.SupplyingMaterialDetails);

                supplyingMaterialsDetails.ForEach(s => s.SupplierId = supplyingMaterialDetailsDTO.SupplierId);

                _unitOfWork.SupplingMaterialDetails.InsertRangeAsync(supplyingMaterialsDetails);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                //throw;
                return StatusCode(500, new ErrorExceptionResponse(500, ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSupplingMaterialToSupplier(int supplierId, [FromBody] List<SupplyingMaterialDetailDTO> supplyingMaterialDetailsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (supplierId <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId);

                if (supplier == null)
                    return BadRequest(new ErrorApiResponse(400, "Supplier is not found"));

                var sentRawMaterialsIds = supplyingMaterialDetailsDTO.Select(rm => rm.MaterialId).ToList();

                var storedRawMaterialsIdsInDb = (await _unitOfWork.RawMaterial.GetAllAsync()).Select(i => i.MaterialId).ToList();

                var invalidRawMaterialsIds = GetInvalidRawMaterialsIdsSend(sentRawMaterialsIds, storedRawMaterialsIdsInDb);

                if (invalidRawMaterialsIds.Count > 0)
                    return BadRequest(new ErrorApiResponse(400, $"The following Materials ids are invalid: [{string.Join(", ", invalidRawMaterialsIds)}]"));

                var supplyingMaterialsDetailsToUpdate = await
                    (_unitOfWork.SupplingMaterialDetails.FindRangeAsync(
                        supplyingDetail => supplyingDetail.SupplierId == supplierId &&
                        supplyingMaterialDetailsDTO.Select(m => m.MaterialId).Contains(supplyingDetail.MaterialId)
                    ));

                if (supplyingMaterialDetailsDTO.Count != supplyingMaterialsDetailsToUpdate.Count())
                    return BadRequest(new ErrorApiResponse(400, $"The entered Material Ids dose not exist in the current supplier Id number : {supplierId}"));

                foreach (var supplyingDetail in supplyingMaterialDetailsDTO)
                {
                    var itemToChange = supplyingMaterialsDetailsToUpdate.FirstOrDefault(d => d.MaterialId == supplyingDetail.MaterialId);

                    if (itemToChange is not null)
                    {
                        _mapper.Map(supplyingDetail, itemToChange);

                        _unitOfWork.SupplingMaterialDetails.Update(itemToChange);
                    }
                }

                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSupplingMaterialsForSupplier(int supplierId, [FromBody] List<SupplierMaterialIdDTO> supplyingMaterialsIds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (supplierId <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId);

                if (supplier == null)
                    return NotFound(new ErrorApiResponse(404, "Supplier Id is not found"));

                var storedRawMaterialsIds = (await _unitOfWork.RawMaterial.GetAllAsync()).Select(i => i.MaterialId).ToList();

                var invalidRawMaterialsIds = GetInvalidRawMaterialsIdsSend(supplyingMaterialsIds.Select(s => s.SupplierMaterialId).ToList(), storedRawMaterialsIds);

                if (invalidRawMaterialsIds.Count > 0)
                    return BadRequest(new ErrorApiResponse(400, $"The following Materials ids are invalid: [{string.Join(", ", invalidRawMaterialsIds)}]"));

                var supplyingMaterialsDetailsToDelete = await
                    (_unitOfWork.SupplingMaterialDetails.FindRangeAsync(
                        supplyingDetail => supplyingDetail.SupplierId == supplierId &&
                        supplyingMaterialsIds.Select(s => s.SupplierMaterialId).ToList()
                        .Contains(supplyingDetail.MaterialId)
                    ));

                if (supplyingMaterialsIds.Count != supplyingMaterialsDetailsToDelete.ToList().Count)
                    return BadRequest(new ErrorApiResponse(400, $"The entered Material Ids does not exist in the current supplier Id number : {supplierId}"));

                _unitOfWork.SupplingMaterialDetails.DeleteRange(supplyingMaterialsDetailsToDelete.ToList());
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSupplingMaterialForSupplier(int supplierId, [FromBody] SupplierMaterialIdDTO supplyingMaterialId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (supplierId <= 0)
                return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId);

                if (supplier == null)
                    return NotFound(new ErrorApiResponse(404, "Supplier Id is not found"));

                var supplyingRawMaterial = await _unitOfWork.RawMaterial.GetByIdAsync(supplyingMaterialId.SupplierMaterialId);

                if (supplyingRawMaterial == null)
                    return BadRequest(new ErrorApiResponse(400, $"The following Material id is invalid: {supplyingMaterialId}"));

                var supplyingMaterialsDetailsToDelete = await
                    (_unitOfWork.SupplingMaterialDetails.FindAsync(
                        s => s.SupplierId == supplierId && supplyingMaterialId.SupplierMaterialId == s.MaterialId
                    ));

                if (supplyingMaterialsDetailsToDelete == null)
                    return BadRequest(new ErrorApiResponse(400, $"The entered Material Ids does not exist in the current supplier Id number : {supplierId}"));

                _unitOfWork.SupplingMaterialDetails.Delete(supplyingMaterialsDetailsToDelete);

                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
            }
        }

        //[HttpDelete]
        //public async Task<IActionResult> DeleteSupplingMaterialForSupplier(int supplierId, int supplyingMaterialId)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    if (supplierId <= 0)
        //        return BadRequest(new ErrorApiResponse(400, "Id can't be 0 or less."));

        //    try
        //    {
        //        var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId);

        //        if (supplier == null)
        //            return NotFound(new ErrorApiResponse(404, "Supplier Id is not found"));

        //        var supplyingRawMaterial = await _unitOfWork.RawMaterial.GetByIdAsync(supplyingMaterialId);

        //        if (supplyingRawMaterial == null)
        //            return BadRequest(new ErrorApiResponse(400, $"The following Material id is invalid: {supplyingMaterialId}"));

        //        var supplyingMaterialsDetailsToDelete = await
        //            (_unitOfWork.SupplingMaterialDetails.FindAsync(
        //                s => s.SupplierId == supplierId && supplyingMaterialId == s.MaterialId
        //            ));

        //        if (supplyingMaterialsDetailsToDelete == null)
        //            return BadRequest(new ErrorApiResponse(400, $"The entered Material Ids does not exist in the current supplier Id number : {supplierId}"));

        //        _unitOfWork.SupplingMaterialDetails.Delete(supplyingMaterialsDetailsToDelete);
        //        await _unitOfWork.Save();

        //        return NoContent();

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ErrorExceptionResponse(500, null, ex.Message));
        //    }
        //}

        #endregion

        private static List<int> GetInvalidRawMaterialsIdsSend(List<int> sentRawMaterialsIds, List<int> storedRawMaterialsIdsInDb)
        {
            var invalidRawMaterialsIds = sentRawMaterialsIds
                .Where(sentMaterialId => !storedRawMaterialsIdsInDb.Contains(sentMaterialId))
                .ToList();

            return invalidRawMaterialsIds;
        }
    }
}
