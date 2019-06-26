using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Juntendo.MedPhys.Esapi
{
    public class DvhProtocol
    {
        public Protocol Protocol { get; set; }
        public List<DvhObjectiveViewModel> ObjectiveVMs { get; set; } = new List<DvhObjectiveViewModel>();
        public PlanningItem PlanningItem { get; set; }
        public bool IsPlanSum { get; set; }
        double TotalPrescribedDoseTps { get; set; }
        DoseValue.DoseUnit DoseUnitTps { get; set; }

        //Stored DVH to prevent multiple GetDVHCumulativeData calls
        public Dictionary<DvhDataParameters, DVHData> StoredDvhs { get; set; } = new Dictionary<DvhDataParameters, DVHData>();

        public DvhProtocol(Protocol protocol, PlanningItem planningItem, bool isPlanSum, double totalPrescribedDoseTps, DoseValue.DoseUnit doseUnitTps)
        {
            Protocol = protocol;
            PlanningItem = planningItem;
            IsPlanSum = isPlanSum;
            TotalPrescribedDoseTps = totalPrescribedDoseTps;
            DoseUnitTps = doseUnitTps;

            foreach (var objective in protocol.Objectives)
            {
                ObjectiveVMs.Add(new DvhObjectiveViewModel(objective, planningItem, isPlanSum, totalPrescribedDoseTps, doseUnitTps, StoredDvhs));
            }
        }

        public DvhProtocol(DvhObjectivesViewModel dvhObjectivesViewModel)
        {
            Protocol = dvhObjectivesViewModel.Protocol;
            PlanningItem = dvhObjectivesViewModel.PlanningItem;
            IsPlanSum = dvhObjectivesViewModel.IsPlanSum;
            TotalPrescribedDoseTps = dvhObjectivesViewModel.AssignedTotalPrescribedDoseTps;
            DoseUnitTps = dvhObjectivesViewModel.SystemDoseUnit;

            ObjectiveVMs = dvhObjectivesViewModel.Objectives.ToList();
        }

        public List<string> GetObjectiveTitles()
        {
            List<string> titles = new List<string>();

            foreach (var objectiveVM in ObjectiveVMs)
            {
                var title = objectiveVM.Objective.Title;
                titles.Add(title);
            }

            return titles;
        }

        public static string GetStringCsvRecord(List<string> items, string delimiter=",")
        {
            string record = string.Empty;
            foreach (var item in items.Select((v, i) => new { v, i }))
            {
                if (item.i == 0)
                {
                    record += Quoted(item.v);
                    continue;
                }

                record += delimiter + Quoted(item.v);
            }

            //record += Environment.NewLine;

            return record;
        }
        public string GetDvhIndexCsvRecord(List<string> titles, string delimiter = ",")
        {
            string record = string.Empty;
            if (titles.Count == 0)
            {
                throw new ArgumentException("No items in titles");
            }

            foreach (var item in titles.Select((v, i) => new { v, i }))
            {
                var query = from o in ObjectiveVMs where o.Objective.Title == item.v select o.Objective;
                if (query.Count() != 1)
                {
                    throw new InvalidOperationException($"No or multiple entries for title: {item.v}");
                }

                var value = query.Single().Value;

                if (item.i == 0)
                {
                    record += Quoted(value);
                    continue;
                }

                record += delimiter + Quoted(value);
            }
            //record += Environment.NewLine;
            return record;
        }
        public static string Quoted(double a)
        {
            return "\"" + a.ToString() + "\"";
        }

        public static string Quoted(string s)
        {
            return "\"" + s + "\"";
        }
    }
}
