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
    [RoutePrefix("api/categories")]
    [Authorize(Roles = "IsAShop")]
    public class CategoriesController : ApiController
    {
        private cartDBEntitiesConn db = new cartDBEntitiesConn();

        // GET: api/Categories
        [HttpGet]
        [Route("GetCategories")]
        public IQueryable<Category> GetCategories()
        {

            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);
            
            return db.Categories.Where(u => u.Shop_Code == shopDetail.Shop_Code);
        }

        // GET: api/Categories/5
        [HttpGet]
        [Route("GetCategories/{id:int}")]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> GetCategory(int id)
        {
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            Category category = await db.Categories.SingleAsync(u => u.Shop_Code == shopDetail.Shop_Code && u.CategoryID==id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut]
        [Route("EditCategories/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (id != category.CategoryID || shopDetail.Shop_Code != category.Shop_Code)
            {
                return BadRequest();
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        [Route("SetCategory")]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PostCategory(SetCategoryModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //db.Categories.Add(category);
            //await db.SaveChangesAsync();

            using (cartDBEntitiesConn enteties = new cartDBEntitiesConn())
            {
                try
                {
                    //enteties.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    db.Configuration.ProxyCreationEnabled = false;
                    ShopDetail shopDetail = enteties.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);
                    
                    Category category = new Category()
                    {
                        CategoryName = categoryModel.CategoryName,
                        Description = categoryModel.Description,
                        Shop_Code = shopDetail.Shop_Code
                    };

                    enteties.Categories.Add(category);
                    await enteties.SaveChangesAsync();

                    return CreatedAtRoute("DefaultApi", new { id = category.CategoryID }, category);
                    //return Ok();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        // DELETE: api/Categories/5
        [Route("RemoveCategory/{id:int}")]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Category category = await db.Categories.FindAsync(id);

            string userId = User.Identity.GetUserId();
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (category == null || shopDetail.Shop_Code != category.Shop_Code)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            await db.SaveChangesAsync();

            return Ok(category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Route("IsCategoryExists")]
        private bool CategoryExists(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Categories.Count(e => e.CategoryID == id) > 0;
        }
    }
}