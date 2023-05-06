using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(ID))]
    public class Diagnosis
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ICollection<MedicalActionDiagnosis> MedicalActionDiagnoses { get; set; } = new List<MedicalActionDiagnosis>();

        #region Conversion Operations

        public static implicit operator DiagnosisEntityGrpc(Diagnosis? input)
        {
            if (input == null) return new DiagnosisEntityGrpc();
            return new DiagnosisEntityGrpc()
            {
                Id = input.ID,
                Name = input.Name
            };
        }

        #endregion
    }
}
