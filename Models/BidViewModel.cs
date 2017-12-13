using System.ComponentModel.DataAnnotations;

namespace beltexam.Models {
    public class BidViewModel {

        [Required(ErrorMessage = "Bid Amount is Required.")]
        [Range(0, 1000000000000000000, ErrorMessage = "Bid Amount cannot be 0")]
        public int Amount { get; set; }
    }
}