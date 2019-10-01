using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly BackEndContext _context;
        private readonly IAuthorizationService _authorizationService;

        public ProductsController(BackEndContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            return await _context.Product.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduct(int id, Product product) {
            if (id != product.Id) {
                return BadRequest();
            }
            Product original = await _context.Product.AsNoTracking<Product>().FirstOrDefaultAsync(p => p.Id == id);
            AuthorizationResult authresult = await _authorizationService.AuthorizeAsync(User, original, "ProductOwner");
            if (!authresult.Succeeded) {
                if (User.Identity.IsAuthenticated) {
                    return new ForbidResult();
                } else {
                    return new ChallengeResult();
                }
            }

            _context.Entry(product).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!ProductExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            product.UserName = User.FindFirst(c => c.Type == JwtClaimTypes.Name && c.Issuer == "http://localhost:5000").Value;
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Product>> DeleteProduct(int id) {
            var product = await _context.Product.FindAsync(id);
            AuthorizationResult authresult = await _authorizationService.AuthorizeAsync(User, product, "ProductOwner");
            if (!authresult.Succeeded) {
                return new ForbidResult();
            }

            if (product == null) {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
