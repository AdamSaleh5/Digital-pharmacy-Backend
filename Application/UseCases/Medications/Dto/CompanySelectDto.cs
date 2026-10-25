using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Medications.Dto
{
    public class CompanySelectDto
    {
        public int Id { get; set; }

        public string Company { get; set; }

        public string Agent { get; set; }

        public string AgentsPhoneNumber { get; set; }
    }
}
