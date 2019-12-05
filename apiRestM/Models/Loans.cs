using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiRestM.Models
{
    public class Loans
    {
        public string id { get; set; }
        public DateTime startDate { get; set; }
        public decimal due { get; set; }
        public string city { get; set; }
        public DateTime endDate { get; set; }
    }
}
