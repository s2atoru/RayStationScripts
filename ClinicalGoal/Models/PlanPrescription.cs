using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalGoal.Models
{
    public class PlanPrescription
    {
        public string PlanId { get; set; }
        public double PrescribedDose { get; set; }
        public int NumberOfFractions { get; set; }
    }
}
