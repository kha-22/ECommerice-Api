﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }
        public string UserType { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Column(TypeName = "Date")]
        public DateTime CreatedDate { get; set; }
    }
}
