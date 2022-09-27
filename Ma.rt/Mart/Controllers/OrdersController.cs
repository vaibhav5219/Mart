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
    [Authorize]
    public class OrdersController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/Orders
        [HttpGet]
        [Route("GetOrders")]
        [Authorize(Roles = "IsAShop")]
        public IQueryable<Order> GetOrders()
        {
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.Orders.Where(u => u.Shop_Code == shopDetail.Shop_Code
                                   && u.Order_Status!=5 
                                   && u.Order_Status!=4
                                   && u.Order_Status!=3);
        }

        // GET: api/Orders/5
        [HttpGet]
        [Route("FetchOrders/{id:int}")]
        [ResponseType(typeof(Order))]
        [Authorize(Roles = "IsAShop")]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
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

        // POST: api/Orders  
        [ResponseType(typeof(Order))]
        [Authorize(Roles = "IsACustomer")]
        [Route("PlaceOrder")]
        public async Task<IHttpActionResult> PostOrder(Order order=null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Configuration.ProxyCreationEnabled = false;
            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.Shop_Code == order.Shop_Code);
            Customer customer = db.Customers.SingleOrDefault(c => c.AspNetUserId == userId);

            // We can remove the Item from cart
            // Then Change Order_Status
            //   Order_Status = 1 => Order Placed
            //   Order_Status = 2 => Order Confirmed By Shop Keeper
            //   Order_Status = 3 => Onthe Way
            //   Order_Status = 4 => Delivered
            //   Order_Status = 5 => Cancled

            List<OrderStatus_tbl> orderStatus_Tbl = db.OrderStatus_tbl.ToList();

            Order order1 = new Order()
            {
                Order_Date = DateTime.Now,
                Order_Status = orderStatus_Tbl[0].OrderStatus_Id,     //    Order Placed => 1
                Order_Total = order.Order_Total,
                Customer_Id = customer.AspNetUserId,
                Product_Id = order.Product_Id,
                Shop_Code = order.Shop_Code,
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = order.Order_Id }, order);
        }

        // DELETE: api/Orders/5  =>  Cancel Order
        [Route("CancelOrder/{id:int}")]
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> DeleteOrder(int id, string Shop_Code)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Order order = await db.Orders.FindAsync(id);
            string userId = User.Identity.GetUserId();
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.Shop_Code != Shop_Code)
            {
                return NotFound();
            }
            List<OrderStatus_tbl> orderStatus_Tbl = db.OrderStatus_tbl.ToList();

            order.Order_Status = orderStatus_Tbl[4].OrderStatus_Id;

            //db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        [ResponseType(typeof(Order))]
        [Authorize(Roles = "IsAShop")]
        [Route("ConfirmOrder")]
        public async Task<IHttpActionResult> PostConfirmOrder(int id, string  Shop_Code)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Order order = await db.Orders.FindAsync(id);
            string userId = User.Identity.GetUserId();
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.Shop_Code != Shop_Code)
            {
                return NotFound();
            }
            order.Order_Status = 2;

            //db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        [ResponseType(typeof(Order))]
        [Authorize(Roles = "IsAShop")]
        [Route("OrderOnTheWay")]
        public async Task<IHttpActionResult> PostOrderOnTheWay(int id, string Shop_Code)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Order order = await db.Orders.FindAsync(id);
            string userId = User.Identity.GetUserId();
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.Shop_Code != Shop_Code)
            {
                return NotFound();
            }
            order.Order_Status = 3;

            //db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        [ResponseType(typeof(Order))]
        [Authorize(Roles = "IsAShop")]
        [Route("OrderDelivered")]
        public async Task<IHttpActionResult> PostOrderDelivered(int id, string Shop_Code)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Order order = await db.Orders.FindAsync(id);
            string userId = User.Identity.GetUserId();
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.Shop_Code != Shop_Code)
            {
                return NotFound();
            }

            List<OrderStatus_tbl> orderStatus_Tbl = db.OrderStatus_tbl.ToList();

            order.Delivered_Date = DateTime.Now;
            order.Order_Status = orderStatus_Tbl[3].OrderStatus_Id;    // Order Delivered  => 4

            //db.Orders.Remove(order);
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
        [Route("IsOrderExists")]
        private bool OrderExists(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Orders.Count(e => e.Order_Id == id) > 0;
        }
    }
}