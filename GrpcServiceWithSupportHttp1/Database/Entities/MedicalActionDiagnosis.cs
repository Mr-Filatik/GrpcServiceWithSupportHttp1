using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(DiagnosisID), nameof(MedicalActionID))]
    public class MedicalActionDiagnosis
    {
        [ForeignKey(nameof(Diagnosis))]
        public int DiagnosisID { get; set; }
        public Diagnosis Diagnosis { get; set; }
        [ForeignKey(nameof(MedicalAction))]
        public int MedicalActionID { get; set; }
        public MedicalAction MedicalAction { get; set; }

        [ForeignKey(nameof(SymptomMeaning))]
        public int? SymptomMeaningID { get; set; }
        public SymptomMeaning? SymptomMeaning { get; set; }

        public int? Order { get; set; }
    }
}
