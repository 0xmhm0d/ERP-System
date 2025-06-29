using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.IServices;
using ERP_Domians.Models;
using ERP_Domians.Models.HelpersParameters;
using ERP_Domians.Models.OwnedProperties;

namespace ERP_BusinessLogic.Services
{
    public class SupplierOrderService : ISupplierOrderService
    {

        private readonly IUnitOfWork _unitOfWork;
        public SupplierOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TbOrder_Supplier> CreateSupplierOrder(int supplierId, decimal shippingCost, List<MaterialsOrderedParameters> materialsOrdered)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId);

            //get Supplier supplying Materials Details
            var supplierSupplyingMaterials = await
                _unitOfWork.SupplingMaterialDetails.FindRangeAsync(
                    supplyingDetails => supplyingDetails.SupplierId == supplierId &&
                    materialsOrdered.Select(m => m.MaterialId).Contains(supplyingDetails.MaterialId)
                );

            //get RawMaterials details
            var rawMaterials = await
                _unitOfWork.RawMaterial.FindRangeAsync(
                    material => materialsOrdered.Select(m => m.MaterialId).Contains(material.MaterialId)
                );

            var ordererDetailsSupplierList = new List<TbOrderDetails_Supplier>();

            foreach (var material in materialsOrdered)
            {
                var supplierMaterialDetails = supplierSupplyingMaterials.FirstOrDefault(s => s.MaterialId == material.MaterialId);

                var rawMaterial = rawMaterials.FirstOrDefault(m => m.MaterialId == material.MaterialId);

                var orderedMaterialDetail = new OrderedRawMaterialDetails(rawMaterial.MaterialId, rawMaterial.MaterialName, supplierMaterialDetails.PricePerUnit);

                var ordererDetailsSupplier = new TbOrderDetails_Supplier(orderedMaterialDetail, material.Qty, material.Qty * supplierMaterialDetails.PricePerUnit);

                ordererDetailsSupplierList.Add(ordererDetailsSupplier);
            }

            var subTotalPrice = ordererDetailsSupplierList.Sum(p => p.Price);

            var totalQty = ordererDetailsSupplierList.Sum(p => p.Quantity);

            var order = new TbOrder_Supplier(supplierId, totalQty, subTotalPrice, shippingCost, subTotalPrice + shippingCost,
                                1, DateTime.Now.AddDays(supplier.AdverageDeliveryTimeInDays), ordererDetailsSupplierList);

            _unitOfWork.OrderSupplier.InsertAsync(order);
            var result = await _unitOfWork.Save();

            return result == 0 ? null : order;
        }
    }
}
