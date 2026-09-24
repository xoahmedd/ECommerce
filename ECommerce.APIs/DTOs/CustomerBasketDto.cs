using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Entities.Basket;

namespace ECommerce.APIs.DTOs
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        public List<BasketItemDto> Items { get; set; } = null!;
    }
}

