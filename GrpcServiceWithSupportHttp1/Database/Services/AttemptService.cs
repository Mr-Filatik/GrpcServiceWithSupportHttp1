using Google.Rpc;
using GrpcServiceWithSupportHttp1.Database.Entities;
using GrpcServiceWithSupportHttp1.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GrpcServiceWithSupportHttp1.Database.Services
{
    public class AttemptService : IAttemptService
    {
        private readonly int _selectCount = 10;
        private readonly ILogger<AttemptService> _logger;
        private readonly DatabaseContext _dbContext;

        public AttemptService(ILogger<AttemptService> logger,
                           DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Attempt> GetAttempt(string code)
        {
            Attempt? attempt = _dbContext.Attempts
                .Include(o => o.User)
                .Include(o => o.Creator)
                .Include(o => o.ExpectedDiagnosis)
                .Include(o => o.SpecifiedDiagnosis)
                .Include(o => o.Mode)
                //.Include(o => o.MedicalActionDiagnoses) //dont work
                    //.ThenInclude(o => o.MedicalAction) //dont work
                .FirstOrDefault(x => x.Code == code/* && x.StartDateTime == null && x.EndDateTime == null*/);
            if (attempt == null) { throw new Exception("Attempt not found"); }
            attempt.MedicalActionDiagnoses = _dbContext.MedicalActionDiagnoses
                .Include(o => o.MedicalAction)
                    .ThenInclude(o => o.Symptom)
                .Include(o => o.SymptomMeaning)
                    .ThenInclude(o => o.Symptom)
                .Where(x => x.DiagnosisID == attempt.ExpectedDiagnosisID && x.Order != null).ToList();
            return attempt;
        }

        public async Task<(Symptom, List<SymptomMeaning>)> GetSymptomById(int id)
        {
            Symptom? symptom = _dbContext.Symptoms.FirstOrDefault(x => x.ID == id);
            if (symptom == null) { throw new Exception("Attempt not found"); }
            List<SymptomMeaning> symptomMeanings = _dbContext.SymptomMeanings.Where(x => x.SymptomID == symptom.ID).ToList();
            return (symptom, symptomMeanings);
        }

        public async Task<List<(Symptom, List<SymptomMeaning>)>> GetSymptoms()
        {
            List<(Symptom, List<SymptomMeaning>)> outputs = new List<(Symptom, List<SymptomMeaning>)> ();
            List<Symptom> symptoms = _dbContext.Symptoms.ToList();
            List<SymptomMeaning> symptomMeanings = new List<SymptomMeaning>();
            for (int i = 0; i < symptoms.Count; i++)
            {
                symptomMeanings = _dbContext.SymptomMeanings.Where(x => x.SymptomID == symptoms[i].ID).ToList();
                outputs.Add((symptoms[i], symptomMeanings));
            }
            return outputs;
        }

        public async Task<Attempt> StartAttempt(int id)
        {
            Attempt? attempt = _dbContext.Attempts
                .FirstOrDefault(x => x.ID == id && x.StartDateTime == null && x.EndDateTime == null);
            if (attempt == null) { throw new Exception("Attempt not found"); }
            attempt.StartDateTime = DateTime.Now.AddHours(4);
            _dbContext.Attempts.Update(attempt);
            _dbContext.SaveChanges();
            attempt = _dbContext.Attempts
                .FirstOrDefault(x => x.ID == id && x.StartDateTime != null && x.EndDateTime == null);
            if (attempt == null) { throw new Exception("Attempt update error"); }
            return attempt;
        }

        public async Task<Attempt> EndAttempt(int id, int diagnosisId, int errorCount)
        {
            Attempt? attempt = _dbContext.Attempts
                .FirstOrDefault(x => x.ID == id && x.StartDateTime != null && x.EndDateTime == null);
            if (attempt == null) { throw new Exception("Attempt not found"); }
            Diagnosis? diagnosis = _dbContext.Diagnoses.FirstOrDefault(x => x.ID == diagnosisId);
            if (diagnosis == null) { throw new Exception("Diagnosis not found"); }
            attempt.EndDateTime = DateTime.Now;
            attempt.SpecifiedDiagnosisID = diagnosis.ID;
            attempt.SpecifiedDiagnosis = diagnosis;
            attempt.ErrorCount = errorCount;
            _dbContext.Attempts.Update(attempt);
            _dbContext.SaveChanges();
            attempt = _dbContext.Attempts
                .FirstOrDefault(x => x.ID == id && x.StartDateTime != null && x.EndDateTime != null);
            if (attempt == null) { throw new Exception("Attempt update error"); }
            return attempt;
        }

        public async Task<Attempt> ResetAttempt(int id)
        {
            Attempt? attempt = _dbContext.Attempts
                .FirstOrDefault(x => x.ID == id);
            if (attempt == null) { throw new Exception("Attempt not found"); }
            attempt.StartDateTime = null;
            attempt.EndDateTime = null;
            _dbContext.Attempts.Update(attempt);
            _dbContext.SaveChanges();
            attempt = _dbContext.Attempts
                .FirstOrDefault(x => x.ID == id && x.StartDateTime == null && x.EndDateTime == null);
            if (attempt == null) { throw new Exception("Attempt update error"); }
            return attempt;
        }

        public async Task<(List<Diagnosis>diagnoses, int currPage, int lastPage)> GetDiagnoses(int page)
        {
            int count = _dbContext.Diagnoses.Count();
            if (page < 1) page = 1;
            int lastPage = (count % _selectCount) == 0 ? (count / _selectCount) : ((count / _selectCount) + 1);
            if (page > lastPage) page = lastPage;

            List<Diagnosis> diagnosisList = _dbContext.Diagnoses.Skip((page - 1) * _selectCount).Take(_selectCount).ToList();
            if (!diagnosisList.Any()) throw new Exception("Diagnoses not found");
            return (diagnosisList, page, lastPage);
        }
    }
}