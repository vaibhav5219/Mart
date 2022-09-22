using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EF.mart;

namespace Mart.Controllers
{
    [RoutePrefix("api/Products")]
    [Authorize(Roles = "IsAShop")]
    public class ProductsController : ApiController
    {
        // GET api/values
        [Route("/GetProducts")]
        public IEnumerable<Product> Get()
        {
            using (cartDBEntitiesConn enteties = new cartDBEntitiesConn())
            {
                try
                {
                    enteties.Configuration.ProxyCreationEnabled = false;
                    List<Product> products = enteties.Products.ToList();
                    return products;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            //return new string[] { "value1", "value2" };
        }
    }
    // set product
    // Edit Product
    // Delete Product

}