using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Authentication.Request
{
    public class CreateUserRequest
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public UserRole Role { get; set; }

        public string? ActivationCode { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? AgentName { get; set; }

        public int? CountryId { get; set; }
    }
}
