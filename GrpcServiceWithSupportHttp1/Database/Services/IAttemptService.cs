using GrpcServiceWithSupportHttp1.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrpcServiceWithSupportHttp1.Database.Services
{
    public interface IAttemptService
    {
        public Task<Attempt> GetAttempt(string code);
        public Task<List<(Symptom, List<SymptomMeaning>)>> GetSymptoms();
        public Task<(Symptom, List<SymptomMeaning>)> GetSymptomById(int id);
        public Task<Attempt> StartAttempt(int id);
        public Task<Attempt> EndAttempt(int id, int diagnosisId, int errorCount);
        public Task<Attempt> ResetAttempt(int id);
        public Task<(List<Diagnosis> diagnoses, int currPage, int lastPage)> GetDiagnoses(int page);
    }
}
