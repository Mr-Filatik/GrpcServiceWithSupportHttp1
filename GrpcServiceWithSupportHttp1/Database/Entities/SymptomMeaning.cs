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

        public ICollection<MedicalActionDiagnosis> MedicalActionDiagnoses { get; set; } = new List<MedicalActionDiagnosis>();

        #region Conversion Operations

        public static implicit operator SymptomMeaningEntityGrpc(SymptomMeaning? input)
        {
            if (input == null) return new SymptomMeaningEntityGrpc();
            return new SymptomMeaningEntityGrpc()
            {
                Id = input.ID,
                Symptom = input.Symptom,
                IsFlag = input.Flag != null,
                Flag = input.Flag != null ? input.Flag.Value : default,
                IsNumber = input.Number != null,
                Number = input.Number != null ? input.Number.Value : default,
                IsValue = input.Value != null,
                Value = input.Value != null ? input.Value : "",
                IsImage = input.Image != null,
                Image = input.Image != null ? input.Image : "",
                IsSound = input.Sound != null,
                Sound = input.Sound != null ? input.Sound : "",
            };
        }

        #endregion
    }
}
