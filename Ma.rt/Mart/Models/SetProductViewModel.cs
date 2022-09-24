using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EF.mart;

namespace Mart.Models
{
    public class SetProductViewModel
    {
        //public string Shop_Code { get; set; }

        public int CategoryID { get; set; }

        public Product Product { get; set; }
    }
}