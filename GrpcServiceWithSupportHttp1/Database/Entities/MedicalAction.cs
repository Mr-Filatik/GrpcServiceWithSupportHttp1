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

        public ICollection<MedicalActionDiagnosis> MedicalActionDiagnoses { get; set; } = new List<MedicalActionDiagnosis>();

        #region Conversion Operations

        public static implicit operator MedicalActionEntityGrpc(MedicalAction? input)
        {
            if (input == null) return new MedicalActionEntityGrpc();
            return new MedicalActionEntityGrpc()
            {
                Id = input.ID,
                Name = input.Name,
                Exercise = input.Exercise,
                Symptom = input.Symptom
            };
        }

        #endregion
    }
}
