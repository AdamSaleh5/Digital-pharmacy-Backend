using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Domain.Entities
{
    public class User : IDataEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public UserRole Role { get; set; }

        public string? ActivationCode { get; set; }

        public string? Email { get; set; } 

        public string? Password { get; set; }

        public string? AgentName  { get; set; }

        public int? CountryId { get; set; }

        public Country? Country { get; set; }

        public string? DeviceIp { get; set; }
    }
}
