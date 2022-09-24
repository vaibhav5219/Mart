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
using Mart.Models;
using Microsoft.AspNet.Identity;

namespace Mart.Controllers
{
    [RoutePrefix("api/Products")]
    [Authorize(Roles = "IsAShop")]
    public class ProductsController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/Products
        // GET api/Products/GetProducts
        [HttpGet]
        [Route("/GetProducts")]
        public IQueryable<Product> GetProducts()
        {
            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.Products.Where(u => u.Shop_Code == shopDetail.Shop_Code);
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id, string Shop_Code)
        {
            Product product = await db.Products.FindAsync(id);
            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (product == null || shopDetail.Shop_Code != Shop_Code)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (id != product.ProductID || shopDetail.Shop_Code != product.Shop_Code)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST:  api/Products/SetProduct
        [HttpPost]
        [Route("/SetProduct")]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(SetProductViewModel setProductViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //db.Products.Add(product);
            //await db.SaveChangesAsync();
            //return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);

            using (cartDBEntitiesConn enteties = new cartDBEntitiesConn())
            {
                try
                {
                    //enteties.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    ShopDetail shopDetail = enteties.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);
                    Product product = new Product()
                    {
                        ProductName = setProductViewModel.Product.ProductName,
                        Description = setProductViewModel.Product.Description,
                        ImagePath = setProductViewModel.Product.ImagePath,      // Need to more work on it
                        UnitPrice = setProductViewModel.Product.UnitPrice,
                        CategoryID = setProductViewModel.CategoryID,
                        Shop_Code = shopDetail.Shop_Code
                    };

                    enteties.Products.Add(product);
                    await enteties.SaveChangesAsync();

                    return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
                    //return Ok();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);

            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (product == null || shopDetail.Shop_Code != product.Shop_Code)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductID == id) > 0;
        }
    }
}