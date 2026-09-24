using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities.Order_Aggregate;

namespace ECommerce.Core.Specification
{
    public class OrderWithItemsAndDeliveryMethodSpecification : BaseSpecification<Order>
    {
        public OrderWithItemsAndDeliveryMethodSpecification(string buyerEmail)
            : base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.DeliveryMethod!);
            Includes.Add(O => O.Items);

            AddOrderByDesc(O => O.OrderDate);
        }

        public OrderWithItemsAndDeliveryMethodSpecification(int orderId, string buyerEmail)
            : base(O => O.Id == orderId && O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.DeliveryMethod!);
            Includes.Add(O => O.Items);
        }
    }
}

