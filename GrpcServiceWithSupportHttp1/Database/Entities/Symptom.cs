using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(ID))]
    public class Symptom
    {
        public int ID { get; set; }

        public string Name { get; set; }

        #region Conversion Operations

        public static implicit operator SymptomEntityGrpc(Symptom? input)
        {
            if (input == null) return new SymptomEntityGrpc();
            return new SymptomEntityGrpc()
            {
                Id = input.ID,
                Name = input.Name,
            };
        }

        #endregion
    }
}
