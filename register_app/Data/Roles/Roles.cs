using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace register_app.Data.Roles
{
    public class Roles
    {
        public static readonly string Admin = "Admin";
        public static readonly string Security = "Security";

        public static readonly string Organiser = "Organiser";

        public static readonly string[] AllRoles = new string[] { Admin, Security, Organiser };
    }
}
