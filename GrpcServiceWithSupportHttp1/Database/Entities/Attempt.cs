using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcServiceWithSupportHttp1.Database.Entities
{
    [PrimaryKey(nameof(ID))]
    public class Attempt
    {
        public int ID { get; set; }

        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Creator))]
        public int CreatorID { get; set; }
        public User Creator { get; set; }
        [ForeignKey(nameof(ExpectedDiagnosis))]
        public int ExpectedDiagnosisID { get; set; } //Ожидаемый диагноз
        public Diagnosis ExpectedDiagnosis { get; set; }
        [ForeignKey(nameof(SpecifiedDiagnosis))]
        public int? SpecifiedDiagnosisID { get; set; } //Указанный диагноз
        public Diagnosis? SpecifiedDiagnosis { get; set; }
        [ForeignKey(nameof(Mode))]
        public ModeEnum ModeID { get; set; }
        public Mode Mode { get; set; }

        [MaxLength(6)]
        public string Code { get; set; } //6-ти значный код для запуска сеанса
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int ErrorCount { get; set; }
    }
}
