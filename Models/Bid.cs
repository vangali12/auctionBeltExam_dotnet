using System;
using System.Collections.Generic;

namespace beltexam.Models
{
    public class Bid: BaseEntity
    {
        public int bidid { get; set; }
        public int Amount { get; set; }
        public int userid { get; set; }
        public User Bidder { get; set; }
        public int productid { get; set; }
        public Product Product { get; set; }
        public Bid()
        {  }
    }

}