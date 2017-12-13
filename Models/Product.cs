using System;
using System.Collections.Generic;

namespace beltexam.Models
{
    public class Product: BaseEntity
    {
        public int productid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Bid { get; set; }
        public DateTime End { get; set; }
        public int userid { get; set; }
        public User Seller { get; set; }
        public Product()
        { }
    }

}