using System;
using System.ComponentModel.DataAnnotations;

namespace beltexam.Models {
    public class MyDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime d = Convert.ToDateTime(value);
            return d >= DateTime.Now;
        }
    }
    public class ProductViewModel {

        [Required(ErrorMessage = "Name is Required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required.")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters.")]
        public string Description { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Bid must be greater than 0")]
        public int Bid { get; set; }

        [Required(ErrorMessage = "End date is Required.")]
        [MyDate(ErrorMessage = "Date must be in the future.")]
        public DateTime End { get; set; }
    }
}