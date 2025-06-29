using System;
using System.ComponentModel.DataAnnotations;

namespace GP_ERP_SYSTEM_v1._0.DTOs
{
    public class AddHRManagerDTO
    {
        [Required(ErrorMessage = "You should insert HR Name")]
        public string HrFullName { get; set; }

        [Required]
        [EmailAddress]
        public string HrEmail { get; set; }

        [Required]
        [Range(minimum: 18, maximum: 65,ErrorMessage ="The age should be between 18 and 65")]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        public decimal Salary { get; set; }

        [Required]
        public string Phone { get; set; }
    }

    public class HRManagerDTO : AddHRManagerDTO
    {
        public int Hrid { get; set; }
    }
}
