using System.Collections.Generic;

namespace GP_ERP_SYSTEM_v1._0.DTOs
{
    public class SupplierMaterialDetailsDTO
    {
        public int SupplierId { get; set; }

        public List<SupplyingMaterialDetailDTO> SupplyingMaterialDetails { get; set; }
    }

    public class SupplierMaterialIdDTO
    {
        public int SupplierMaterialId { get; set; }
    }
}
