using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;

namespace SimpleBillPay.Areas.Identity.Data
{
    public class AuthenticationContext : IdentityDbContext<User>
    {

        public AuthenticationContext() : base()
        {
            
        }
        
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
        }

    }

    public class User : IdentityUser
    {
        public DateTime AccountCreationDateTime { get; set; } 
    }
}
