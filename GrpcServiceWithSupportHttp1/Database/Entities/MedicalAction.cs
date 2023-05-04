using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(ID))]
    public class MedicalAction
    {
        public int ID { get; set; }

        [ForeignKey(nameof(Symptom))]
        public int? SymptomID { get; set; }
        public Symptom? Symptom { get; set; }

        public string Name { get; set; }
        public string Exercise { get; set; }
    }
}
