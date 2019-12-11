using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASM_ASP.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int  MemberId { get; set; }
        public PaymentType PaymentTypeId { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipPhone { get; set; }
        public double TotalPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public OrderStatus Status { get; set; }

        public enum OrderStatus
        {
            Pending=5,condirmed=4,Shipping=3,Paid=2,Done=1,Cancel=0,Delete=-1
        }
        public enum PaymentType
        {
            Cod=1,InternetBanking=2,DirectTransfer=3
        }
        public Order()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Status =  OrderStatus.Pending;
        }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}