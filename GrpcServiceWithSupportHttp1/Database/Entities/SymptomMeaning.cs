using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(ID))]
    public class SymptomMeaning
    {
        public int ID { get; set; }

        [ForeignKey(nameof(Symptom))]
        public int SymptomID { get; set; }
        public Symptom Symptom { get; set; }

        public bool? Flag { get; set; }
        public int? Number { get; set; }
        public string? Value { get; set; }
        public string? Image { get; set; }
        public string? Sound { get; set; }
    }
}
