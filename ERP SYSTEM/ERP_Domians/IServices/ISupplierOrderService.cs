using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_Domians.Models;
using ERP_Domians.Models.HelpersParameters;

namespace ERP_Domians.IServices
{
    public interface ISupplierOrderService
    {
        public Task<TbOrder_Supplier> CreateSupplierOrder(int supplierId, decimal shippingCost, List<MaterialsOrderedParameters> materialsOrdered);
    }
}
