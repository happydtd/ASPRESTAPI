using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Models
{
    public class EmployeeUpdateDto
    {
        [Display(Name = "yuangonghao")]
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(10, ErrorMessage = "{0} max length is {1}")]
        public string EmployeeNo { get; set; }

        [Display(Name = "ming")]
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(50, ErrorMessage = "{0} max length is {1}")]
        public string FirstName { get; set; }

        [Display(Name = "xin")]
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(50, ErrorMessage = "{0} max length is {1}")]
        public string LastName { get; set; }

        [Display(Name = "sex")]
        public string Gender { get; set; }

        [Display(Name = "birthday")]
        public string DateOfBirth { get; set; }
    }
}
