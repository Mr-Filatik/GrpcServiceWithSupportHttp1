using GrpcServiceWithSupportHttp1.Database.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Cryptography;

namespace GrpcServiceWithSupportHttp1.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<MedicalAction> MedicalActions { get; set; }
        public DbSet<SymptomMeaning> SymptomMeanings { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<MedicalActionDiagnosis> MedicalActionDiagnoses { get; set; }
        public DbSet<Mode> Modes { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
 
        public DatabaseContext(DbContextOptions<DatabaseContext> optionsBuilder) : base(optionsBuilder)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Role> roles = new List<Role>()
            {
                new Role() 
                { 
                    ID = RoleEnum.Student, 
                    Name = RoleEnum.Student.ToString() 
                },
                new Role() 
                { 
                    ID = RoleEnum.Instructor, 
                    Name = RoleEnum.Instructor.ToString() 
                },
                new Role() 
                { 
                    ID = RoleEnum.Administrator, 
                    Name = RoleEnum.Administrator.ToString() 
                }
            };
            modelBuilder.Entity<Role>().HasData(roles);

            List<Mode> modes = new List<Mode>()
            {
                new Mode() { ID = ModeEnum.Education, Name = ModeEnum.Education.ToString() },
                new Mode() { ID = ModeEnum.Testing, Name = ModeEnum.Testing.ToString() },
                new Mode() { ID = ModeEnum.Exam, Name = ModeEnum.Exam.ToString() }
            };
            modelBuilder.Entity<Mode>().HasData(modes);

            List<User> users = new List<User>() {
                new User() { Id = 1, Email = "filatov23wlad@gmail.com", PasswordHash = PasswordHash("password"), Surname = "Филатов", Name = "Владислав", Patronymic = "Алексеевич", Birthday = new DateTime(1999, 6, 23), AccessKey = null },
                new User() { Id = 2, Email = "ermakov.grisha@yandex.ru ", PasswordHash = PasswordHash("password"), Surname = "Ермаков", Name = "Григорий", Patronymic = "Евгеньевич", Birthday = new DateTime(2000, 2, 1), AccessKey = null },
                new User() { Id = 3, Email = "instructor@mail.ru", PasswordHash = PasswordHash("password"), Surname = "Основной", Name = "Инструктор", Patronymic = "Приложения", Birthday = new DateTime(2001, 5, 16), AccessKey = null },
                new User() { Id = 4, Email = "student@mail.ru", PasswordHash = PasswordHash("password"), Surname = "Основной", Name = "Ученик", Patronymic = "Приложения", Birthday = new DateTime(2002, 7, 28), AccessKey = null },
            };
            modelBuilder.Entity<User>().HasData(users);

            List<Symptom> symptoms = new List<Symptom>()
            {
                new Symptom() { ID = 1, Name = "Наличие сознания"},
                new Symptom() { ID = 2, Name = "Проходимость дыхательных путей"},
                new Symptom() { ID = 3, Name = "Сатурация крови кислородом"},
                new Symptom() { ID = 4, Name = "Частота дыхательной системы"},
                new Symptom() { ID = 5, Name = "Пульс"}
            };
            modelBuilder.Entity<Symptom>().HasData(symptoms);

            List<MedicalAction> medicalActions = new List<MedicalAction>()
            {
                new MedicalAction() { ID = 1, SymptomID = 1, Name = "Проверка наличия сознания", Exercise = "Проверьте наличие сознания у пациента" },
                new MedicalAction() { ID = 2, SymptomID = 2, Name = "Оценка проходимости дыхательных путей", Exercise = "Оцените проходимость дыхательных путей у пациента" },
                new MedicalAction() { ID = 3, SymptomID = 3, Name = "Оценка сатурации крови кислородом", Exercise = "Оцените уровень сатурации крови кислородом у пациента" },
                new MedicalAction() { ID = 4, SymptomID = 4, Name = "Оценка частоты дыхания", Exercise = "Оцените частоту дыхания у пациента" },
                new MedicalAction() { ID = 5, SymptomID = 5, Name = "Оценка частоты пульса", Exercise = "Оцените частоту пульса у пациента" },
                new MedicalAction() { ID = 6, SymptomID = null, Name = "Надевание стерильных перчаток", Exercise = "Наденьте стерильные перчатки" }
            };
            modelBuilder.Entity<MedicalAction>().HasData(medicalActions);

            List<SymptomMeaning> symptomMeanings = new List<SymptomMeaning>()
            {
                new SymptomMeaning() { ID = 1, SymptomID = 1, Flag = true, Number = null, Value = null, Image = null, Sound = null },
                new SymptomMeaning() { ID = 2, SymptomID = 2, Flag = null, Number = null, Value = "Чистые", Image = null, Sound = null },
                new SymptomMeaning() { ID = 3, SymptomID = 2, Flag = null, Number = null, Value = "Грязные", Image = null, Sound = null }
            };
            modelBuilder.Entity<SymptomMeaning>().HasData(symptomMeanings);

            List<Diagnosis> diagnoses = new List<Diagnosis>()
            {
                new Diagnosis() { ID = 1, Name = "Неизвестный диагноз" },
                new Diagnosis() { ID = 2, Name = "Бронхообструктивный синдром" }
            };
            modelBuilder.Entity<Diagnosis>().HasData(diagnoses);

            modelBuilder.Entity<MedicalActionDiagnosis>().HasOne(x => x.MedicalAction).WithMany(x => x.MedicalActionDiagnoses);
            modelBuilder.Entity<MedicalActionDiagnosis>().HasOne(x => x.SymptomMeaning).WithMany(x => x.MedicalActionDiagnoses);
            modelBuilder.Entity<MedicalActionDiagnosis>().HasOne(x => x.Diagnosis).WithMany(x => x.MedicalActionDiagnoses);
            List<MedicalActionDiagnosis> medicalActionDiagnoses = new List<MedicalActionDiagnosis>()
            {
                new MedicalActionDiagnosis() { DiagnosisID = 2, MedicalActionID = 1, SymptomMeaningID = 1, Order = 1 },
                new MedicalActionDiagnosis() { DiagnosisID = 2, MedicalActionID = 2, SymptomMeaningID = 3, Order = 2 },
                new MedicalActionDiagnosis() { DiagnosisID = 2, MedicalActionID = 6, SymptomMeaningID = null, Order = 3 }
            };
            modelBuilder.Entity<MedicalActionDiagnosis>().HasData(medicalActionDiagnoses);

            modelBuilder.Entity<Attempt>().HasOne(x => x.Creator).WithMany(x => x.CreatedAttempts).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Attempt>().HasOne(x => x.User).WithMany(x => x.AvailableAttempts).OnDelete(DeleteBehavior.NoAction);
            List<Attempt> attempts = new List<Attempt>()
            {
                new Attempt() { ID = 1, ModeID = ModeEnum.Education, Code = "EDEDED", CreatorID = 3, UserID = 4, ExpectedDiagnosisID = 2, StartDateTime = null, EndDateTime = null, SpecifiedDiagnosisID = null, ErrorCount = 0 },
                new Attempt() { ID = 2, ModeID = ModeEnum.Testing, Code = "TETETE", CreatorID = 3, UserID = 4, ExpectedDiagnosisID = 2, StartDateTime = null, EndDateTime = null, SpecifiedDiagnosisID = null, ErrorCount = 0 },
                new Attempt() { ID = 3, ModeID = ModeEnum.Exam, Code = "EXEXEX", CreatorID = 3, UserID = 4, ExpectedDiagnosisID = 2, StartDateTime = null, EndDateTime = null, SpecifiedDiagnosisID = null, ErrorCount = 0 }
            };
            modelBuilder.Entity<Attempt>().HasData(attempts);
        }

        public string PasswordHash(string passwordString)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: passwordString,
                salt: RandomNumberGenerator.GetBytes(128 / 8),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
    }
}
