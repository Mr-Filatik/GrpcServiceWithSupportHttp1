using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(ID))]
    public class Mode
    {
        public ModeEnum ID { get; set; }

        [MaxLength(15)]
        public string Name { get; set; }

        #region Conversion Operations

        public static implicit operator ModeEntityGrpc(Mode input)
        {
            return new ModeEntityGrpc()
            {
                Id = (int)input.ID,
                Name = input.Name
            };
        }

        public static implicit operator Mode(ModeEntityGrpc input)
        {
            return new Mode()
            {
                ID = (ModeEnum)input.Id,
                Name = input.Name
            };
        }

        #endregion
    }

    public enum ModeEnum
    {
        Education = 1,
        Testing = 2,
        Exam = 3
    }
}
