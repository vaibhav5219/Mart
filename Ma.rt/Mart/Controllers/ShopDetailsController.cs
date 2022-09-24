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
    [RoutePrefix("api/ShopDetails/")]
    [Authorize(Roles = "IsAShop")]
    public class ShopDetailsController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/ShopDetails
        //public IQueryable<ShopDetail> GetShopDetails()
        //{
        //    return db.ShopDetails;
        //}

        // GET: api/ShopDetails/5
        [Route("GetShopDetails/{id}")]
        [ResponseType(typeof(ShopDetail))]
        public async Task<IHttpActionResult> GetShopDetail(string id)
        {
            ShopDetail shopDetail = await db.ShopDetails.FindAsync(id);

            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail1 = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);


            if (shopDetail == null || shopDetail.Shop_Code != shopDetail1.Shop_Code)
            {
                return NotFound();
            }

            return Ok(shopDetail);
        }

        // PUT: api/ShopDetails/5
        [Route("EditShopDetails/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutShopDetail(string id, ShopDetail shopDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ShopDetail shopDetail1 = await db.ShopDetails.FindAsync(id);

            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail2 = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (id != shopDetail.Shop_Id || shopDetail1.Shop_Code != shopDetail2.Shop_Code)
            {
                return BadRequest();
            }

            db.Entry(shopDetail).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopDetailExists(id))
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

        // POST: api/ShopDetails
        //[ResponseType(typeof(ShopDetail))]
        //public async Task<IHttpActionResult> PostShopDetail(ShopDetail shopDetail)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.ShopDetails.Add(shopDetail);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (ShopDetailExists(shopDetail.Shop_Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = shopDetail.Shop_Id }, shopDetail);
        //}

        // DELETE: api/ShopDetails/5
        //[ResponseType(typeof(ShopDetail))]
        //public async Task<IHttpActionResult> DeleteShopDetail(string id)
        //{
        //    ShopDetail shopDetail = await db.ShopDetails.FindAsync(id);
        //    if (shopDetail == null)
        //    {
        //        return NotFound();
        //    }

        //    db.ShopDetails.Remove(shopDetail);
        //    await db.SaveChangesAsync();

        //    return Ok(shopDetail);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShopDetailExists(string id)
        {
            return db.ShopDetails.Count(e => e.Shop_Id == id) > 0;
        }
    }
}