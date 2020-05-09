using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Juntendo.MedPhys
{
    public class Protocol
    {
        public string Id { get; private set; }
        public string FilePath { get; private set; }
        public List<DvhObjective> Objectives { get; private set; }

        public Protocol(string id, string filePath, List<DvhObjective> objectives)
        {
            Id = id;
            FilePath = filePath;
            Objectives = objectives;
        }

        public Protocol(string id, string filePath)
        {
            Id = id;
            FilePath = filePath;
            Objectives = DvhObjective.ReadObjectivesFromCsv(filePath);
        }

        public static List<Protocol> GetProtocolsFromFolder(string folderPath)
        {
            var protocols = new List<Protocol>();
            var filePaths = Directory.GetFiles(folderPath, "*.csv");

            foreach (string filePath in filePaths)
            {
                var objectives = DvhObjective.ReadObjectivesFromCsv(filePath);
                var id = (objectives.First()).ProtocolId;
                var protocol = new Protocol(id, filePath, objectives);
                protocols.Add(protocol);
            }

            return protocols;
        }
    }
}
