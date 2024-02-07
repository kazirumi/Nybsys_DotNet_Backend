using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet6Authorization.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Column(TypeName ="nvarchar(150)")]
        public string? FullName { get; set; }

        [NotMapped]
        public string Role { get; set; }

        //public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();

    }
}
