using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ValidationResult<T>
    {
        public bool Status { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<T> Records { get; set; } = new List<T>();
    }
}
