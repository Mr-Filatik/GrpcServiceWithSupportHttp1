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
    }

    public enum ModeEnum
    {
        Education = 1,
        Testing = 2,
        Exam = 3
    }
}
