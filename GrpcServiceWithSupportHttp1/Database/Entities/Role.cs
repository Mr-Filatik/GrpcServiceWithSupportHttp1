using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(ID))]
    public class Role
    {
        public RoleEnum ID { get; set; }

        [MaxLength(15)]
        public string Name { get; set; }
    }

    public enum RoleEnum
    {
        Student = 1,
        Instructor = 2,
        Administrator = 3
    }
}
