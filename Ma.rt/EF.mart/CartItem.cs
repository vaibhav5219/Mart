//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EF.mart
{
    using System;
    using System.Collections.Generic;
    
    public partial class CartItem
    {
        public int Quantity { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int ProductId { get; set; }
        public string Cart_Id { get; set; }
        public string Item_Id { get; set; }
        public string Shop_Code { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
