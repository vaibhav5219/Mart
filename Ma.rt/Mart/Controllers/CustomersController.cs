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
    [Authorize(Roles = "IsACustomer")]
    [RoutePrefix("api/Customers")]
    public class CustomersController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/Customers
        //[Route("GetCustomers")]
        //public IQueryable<Customer> GetCustomers()
        //{
        //    return db.Customers;
        //}

        // GET: api/Customers/5
        [Route("GetCustomer/{id:int}")]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer1 = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null || customer1.UserName != customer.UserName)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        [Route("UpdateCustomer/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Configuration.ProxyCreationEnabled = false;
            string userId = User.Identity.GetUserId();
            Customer customer1 = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (id != customer.CustomerId || customer.UserName != customer1.UserName)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        //[ResponseType(typeof(Customer))]
        //public async Task<IHttpActionResult> PostCustomer(Customer customer)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Customers.Add(customer);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (CustomerExists(customer.CustomerId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        //}

        // DELETE: api/Customers/5
        //[ResponseType(typeof(Customer))]
        //public async Task<IHttpActionResult> DeleteCustomer(int id)
        //{
        //    Customer customer = await db.Customers.FindAsync(id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Customers.Remove(customer);
        //    await db.SaveChangesAsync();

        //    return Ok(customer);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [Route("IsCustomerExists")]
        private bool CustomerExists(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}