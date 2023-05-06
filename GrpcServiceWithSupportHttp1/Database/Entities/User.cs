using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? AccessKey { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string? Patronymic { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                if (Patronymic == null)
                {
                    return $"{Surname} {Name}";
                }
                return $"{Surname} {Name} {Patronymic}";
            }
        }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [NotMapped]
        public int Age 
        { 
            get 
            {
                DateTime date = DateTime.UtcNow;
                int age = Birthday.Year - date.Year;
                if (Birthday.Month < date.Month || (Birthday.Month == date.Month && Birthday.Day < date.Day))
                {
                    age--;
                }
                return age;
            } 
        }

        public ICollection<Attempt> CreatedAttempts { get; set; } = new List<Attempt>();
        public ICollection<Attempt> AvailableAttempts { get; set; } = new List<Attempt>();

        #region Conversion Operations

        public static implicit operator UserForAttemptEntityGrpc(User? input)
        {
            if (input == null) return new UserForAttemptEntityGrpc();
            return new UserForAttemptEntityGrpc()
            {
                Id = input.Id,
                Surname = input.Surname,
                Name = input.Name,
                Patronymic = input.Patronymic
            };
        }

        #endregion
    }
}
