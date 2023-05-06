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

        #region Conversion Operations

        public static implicit operator MedicalActionDiagnosisEntityGrpc(MedicalActionDiagnosis? input)
        {
            if (input == null) return new MedicalActionDiagnosisEntityGrpc();
            return new MedicalActionDiagnosisEntityGrpc()
            {
                Id = input.DiagnosisID,
                MedicalAction = input.MedicalAction,
                SymptomMeaning = input.SymptomMeaning,
                Order = input.Order != null ? input.Order.Value : default,
            };
        }

        #endregion
    }
}
