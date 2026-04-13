using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class UserAllergen
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int AllergenId { get; set; }
        public Allergen Allergen { get; set; }
    }
}
