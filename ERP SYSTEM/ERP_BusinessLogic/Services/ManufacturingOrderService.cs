using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains.Interfaces.IUnitOfWork;
using ERP_Domians.IServices;
using ERP_Domians.Models;
using ERP_Domians.Models.HelpersParameters;

namespace ERP_BusinessLogic.Services
{
    public class ManufacturingOrderService : IManufacturingOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ManufacturingOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TbManufacturingOrder> CreateManufacturingOrder(int productId, int qty, decimal costs, DateTime startingDate, List<MaterialsOrderedParameters> rawMaterialsUsed)
        {
            var manufacturingOrderDetailsList = new List<TbManufacturingOrderDetail>();

            foreach (var rawMaterial in rawMaterialsUsed)
            {
                var rawMaterialUsed = new TbManufacturingOrderDetail(rawMaterial.MaterialId, rawMaterial.Qty);

                manufacturingOrderDetailsList.Add(rawMaterialUsed);
            }

            var order = new TbManufacturingOrder(productId, qty, startingDate, startingDate.AddDays(3), costs, 1, manufacturingOrderDetailsList);

            _unitOfWork.Manufacturing.InsertAsync(order);

            return await _unitOfWork.Save() == 0 ? null : order;
        }
    }
}
