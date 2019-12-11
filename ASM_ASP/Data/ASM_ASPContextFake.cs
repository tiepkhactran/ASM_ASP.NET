using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ASM_ASP.Data
{
    public class ASM_ASPContextFake : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ASM_ASPContextFake() : base("name=ASM_ASPContextFake")
        {
        }

        public System.Data.Entity.DbSet<ASM_ASP.Models.Product> Products { get; set; }
    }
}
