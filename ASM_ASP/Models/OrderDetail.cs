using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASM_ASP.Models
{
    public class OrderDetail
    {
        [Key,Column(Order=0)]
        public int ProductId { get; set; }
        [Key, Column(Order = 1)]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public int  Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}