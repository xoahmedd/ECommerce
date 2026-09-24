using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.APIs.DTOs;
using ECommerce.APIs.Errors;
using ECommerce.Core.Entities.Basket;
using ECommerce.Core.Reposiroties_Contracts;

namespace ECommerce.APIs.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            return Ok(basket is null ? new CustomerBasket(id) : basket);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);

            var createdOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

            if (createdOrUpdatedBasket is null) return BadRequest(new ApiResponse(400));

            return Ok(createdOrUpdatedBasket);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }
    }
}

