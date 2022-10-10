using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EF.mart;

namespace Mart.Models
{
    public class ShopAccountViewModels
    {
        public RegisterBindingModel RegisterBindingModel { get; set; }

        [Required]
        public string Shop_Code { get; set; }
        [Required]
        public string ShopName { get; set; }
        [Required]
        public string ShopKeeperName { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public int Address { get; set; }
        [Required]
        public string Shop_Domain_Name { get; set; }
        [Required]
        public string Pin_Code { get; set; }
        [Required]
        public string AspNetUsersId { get; set; }
    }
}