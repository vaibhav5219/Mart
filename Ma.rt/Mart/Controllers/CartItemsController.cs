using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EF.mart;
using Microsoft.AspNet.Identity;

namespace Mart.Controllers
{
    [RoutePrefix("api/CartItems")]
    [Authorize]
    public class CartItemsController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/CartItems
        [HttpGet]
        [Route("GetCartItems")]
        public IQueryable<CartItem> GetCartItems()
        {
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.CartItems.Where(u => u.Shop_Code == shopDetail.Shop_Code);
        }

        // GET: api/CartItems/5
        [HttpGet]
        [Route("GetOrders/{id}")]
        [Authorize(Roles = "IsACustomer")]
        [ResponseType(typeof(CartItem))]
        public async Task<IHttpActionResult> GetCartItem(string id)
        {
            CartItem cartItem = await db.CartItems.FindAsync(id);

            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (cartItem == null || shopDetail.Shop_Code != cartItem.Shop_Code)
            {
                return NotFound();
            }

            return Ok(cartItem);
        }

        // PUT: api/CartItems/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutCartItem(string id, CartItem cartItem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != cartItem.Cart_Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(cartItem).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CartItemExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/CartItems  =>  Add to cart
        [Authorize(Roles = "IsACustomer")]
        [Route("AddToCart")]
        [ResponseType(typeof(CartItem))]
        public async Task<IHttpActionResult> PostCartItem(CartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer = db.Customers.FirstOrDefault(c => c.AspNetUserId == userId);

            var cartItemExists = db.CartItems.SingleOrDefault(
               c => c.Cart_Id == cartItem.Cart_Id 
               && c.ProductId == cartItem.ProductId);

            if (cartItemExists == null)
            {
                // Create a new cart item if no cart item exists.                 
                cartItem = new CartItem
                {
                    Item_Id = Guid.NewGuid().ToString(),
                    ProductId = cartItem.ProductId,
                    //Product = db.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                    Shop_Code = cartItem.Shop_Code
                };

                db.CartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Quantity++;
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartItemExists(cartItem.Cart_Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cartItem.Cart_Id }, cartItem);
        }

        // DELETE: api/CartItems/5  => Remove Cart Item
        [ResponseType(typeof(CartItem))]
        [Authorize(Roles = "IsACustomer")]
        [Route("RemoveCartItem/{id:int}")]
        public async Task<IHttpActionResult> DeleteCartItem(string id)
        {
            CartItem cartItem = await db.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer = db.Customers.FirstOrDefault(c => c.AspNetUserId == userId);

            var cartItemExists = db.CartItems.SingleOrDefault(
               c => c.Cart_Id == cartItem.Cart_Id
               && c.ProductId == cartItem.ProductId);

            if (cartItemExists != null && cartItem.Quantity > 0)
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Quantity--;
            }

            db.CartItems.Remove(cartItem);
            await db.SaveChangesAsync();

            return Ok(cartItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Route("IsCartItemExists")]
        private bool CartItemExists(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.CartItems.Count(e => e.Cart_Id == id) > 0;
        }
    }
}