using ECommerice.Core.Entities.OrderAggregate;
using Microsoft.AspNetCore.Identity;
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

        public List<Contactus> ContactusList { get; set; }
        public List<Address> AddressList { get; set; }
        public List<Order> OrderList { get; set; }
    }
}
