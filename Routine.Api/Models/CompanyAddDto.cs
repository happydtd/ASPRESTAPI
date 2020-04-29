using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Models
{
    public class CompanyAddDto
    {
        [Display(Name="gong si ming")]
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(100, ErrorMessage = "{0} max length is {1}")]
        public string Name { get; set; }

        [Display(Name = "jianjie")]
        [StringLength(500, MinimumLength =10, ErrorMessage = "{0} max length is between {2} and {1}")]
        public string Introduction { get; set; }

        public ICollection<EmployeeAddDto> Employees { get; set; } = new List<EmployeeAddDto>();
    }
}
