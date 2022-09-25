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
    [Authorize]
    [RoutePrefix("api/Address")]
    public class AddressesController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/Addresses
        //public IQueryable<Address> GetAddresses()
        //{
        //    return db.Addresses;
        //}

        // GET: api/Addresses/5
        [Route("GetAddress/{id:int}")]
        [ResponseType(typeof(Address))]
        public async Task<IHttpActionResult> GetAddress(int id)
        {
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            Address address = await db.Addresses.FindAsync(id);

            if (address == null || address.CustomerUserName != customer.UserName)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // PUT: api/Addresses/5
        [Route("UpdateAddress/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAddress(int id, Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (id != address.AddressId || address.CustomerUserName != customer.AspNetUserId)
            {
                return BadRequest();
            }

            db.Entry(address).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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

        // POST: api/Addresses
        [Route("PostAddress/{id:int}")]
        [ResponseType(typeof(Address))]
        public async Task<IHttpActionResult> PostAddress(Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (address.CustomerUserName != customer.AspNetUserId)
            {
                return BadRequest();
            }

            db.Addresses.Add(address);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = address.AddressId }, address);
        }

        // DELETE: api/Addresses/5
        [Route("RemoveAddress/{id:int}")]
        [ResponseType(typeof(Address))]
        public async Task<IHttpActionResult> DeleteAddress(int id)
        {
            Address address = await db.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (id != address.AddressId || address.CustomerUserName != customer.AspNetUserId)
            {
                return BadRequest();
            }


            db.Addresses.Remove(address);
            await db.SaveChangesAsync();

            return Ok(address);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AddressExists(int id)
        {
            return db.Addresses.Count(e => e.AddressId == id) > 0;
        }
    }
}