﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GP_ERP_SYSTEM_v1._0.DTOs
{
    public class AddSupplierDTO
    {
        public string SupplierName { get; set; }

        public string SupplierDescription { get; set; }

        [Display(Name = "AverageDeliveryTimeInDays")]
        public int AdverageDeliveryTimeInDays { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }
    }

    public class SupplierDTO : AddSupplierDTO
    {
        public int SupplierId { get; set; }
    }
}
