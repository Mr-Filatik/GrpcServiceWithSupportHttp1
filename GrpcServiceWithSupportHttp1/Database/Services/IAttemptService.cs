using GrpcServiceWithSupportHttp1.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrpcServiceWithSupportHttp1.Database.Services
{
    public interface IAttemptService
    {
        public Task<Attempt> GetAttempt(string code);
        public Task<List<(Symptom, List<SymptomMeaning>)>> GetSymptoms();
        public Task<(Symptom, List<SymptomMeaning>)> GetSymptomById(int id);
    }
}
