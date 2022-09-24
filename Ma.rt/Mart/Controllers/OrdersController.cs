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
    [RoutePrefix("api/Orders")]
    [Authorize(Roles = "IsAShop")]
    public class OrdersController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/Orders
        [HttpGet]
        [Route("/GetOrders")]
        public IQueryable<Order> GetOrders()
        {
            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.Orders.Where(u => u.Shop_Code == shopDetail.Shop_Code);
        }

        // GET: api/Orders/5
        [HttpGet]
        [Route("/GetOrders/{id}")]
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (order == null || order.Shop_Code != shopDetail.Shop_Code)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        //[Route("UpdateOrder/{id}")]
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutOrder(int id, Order order)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != order.Order_Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(order).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrderExists(id))
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

        //// POST: api/Orders
        //[ResponseType(typeof(Order))]
        //public async Task<IHttpActionResult> PostOrder(Order order)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Orders.Add(order);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = order.Order_Id }, order);
        //}

        // DELETE: api/Orders/5  =>  Cancel Order
        [Route("CancelOrder/{id}")]
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (order == null || order.Shop_Code != shopDetail.Shop_Code)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Order_Id == id) > 0;
        }
    }
}