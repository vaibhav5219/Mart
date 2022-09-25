using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EF.mart;

namespace Mart.Models
{
    public class CustomerAccountViewModel
    {
        public RegisterBindingModel RegisterBindingModel { get; set; }

        public Customer Customer { get; set; }
    }
}