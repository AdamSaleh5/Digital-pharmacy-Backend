using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum UserRole
    {
        [PgName("Administrator")]
        Administrator = 1,

        [PgName("Admin")]
        Admin = 2,

        [PgName("Agent")]
        Agent = 3,

        [PgName("User")]
        User = 4
    }
}
