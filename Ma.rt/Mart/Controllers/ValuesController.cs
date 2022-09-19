using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EF.mart;

namespace Mart.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<Category> Get()
        {
            using (cartDBEntitiesConn enteties = new cartDBEntitiesConn())
            {
                try
                {
                    enteties.Configuration.ProxyCreationEnabled = false;
                    List<Category> categories = enteties.Categories.ToList();
                    return categories;
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public HttpResponseMessage Get(int id)
        {
            using (cartDBEntitiesConn ctDB = new cartDBEntitiesConn())
            {
                // List<Categories> categories = ctDB.Categories.ToList();
                var entity = ctDB.Categories.FirstOrDefault(c => c.CategoryID==id);

                if(entity!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category with id : "+id.ToString()+" not fount");
            }
        }

        // POST api/values
        public HttpResponseMessage Post([FromBody] Category category)
        {
            try
            {
                using (cartDBEntitiesConn ctDB = new cartDBEntitiesConn())
                {
                    // List<Categories> categories = ctDB.Categories.ToList();
                    ctDB.Categories.Add(category);
                    ctDB.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, category);

                    message.Headers.Location = new Uri(Request.RequestUri + category.CategoryID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
