using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Authentication.Dto
{
    public class UsersDtos
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? AgentName { get; set; }

        public string? ActivationCode { get; init; }

        public string? Password { get; init; }

        public int? CountryId { get; set; }
    }
}
