using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.APIs.Errors;
using ECommerce.Repository.Data;

namespace ECommerce.APIs.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")] // GET : /api/Buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbContext.Products.Find(100);

            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(product);
        }

        [HttpGet("badrequest")] // GET : /api/Buggy/badrequest
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("unauthorized")] // GET : /api/Buggy/unauthorized
        public ActionResult GetUnauthorizedError()
        {
            return Unauthorized(new ApiResponse(401));
        }

        [HttpGet("badrequest/{id}")] // GET : /api/Buggy/badrequest/five
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }

        [HttpGet("servererror")] // GET : /api/Buggy/servererror
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(100);

            var productToReturn = product.ToString();

            return Ok(productToReturn);
        }

    }
}

