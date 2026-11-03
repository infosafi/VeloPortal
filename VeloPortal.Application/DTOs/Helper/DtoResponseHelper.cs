using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Helper
{
    public class DtoResponseHelper
    {
        public bool IsSuccess { get; set; }
        public string message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
