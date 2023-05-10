using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServiceWithSupportHttp1.Database;
using GrpcServiceWithSupportHttp1.Database.Entities;
using GrpcServiceWithSupportHttp1.Database.Services;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Generic;

namespace GrpcServiceWithSupportHttp1.Services
{
    public class AttemptGrpcService : AttemptGrpc.AttemptGrpcBase
    {
        private readonly ILogger<AttemptGrpcService> _logger;
        private readonly IAttemptService _attemptService;

        public AttemptGrpcService(
            ILogger<AttemptGrpcService> logger,
            IAttemptService userService)
        {
            _logger = logger;
            _attemptService = userService;
        }

        public override Task<LoadAttemptResponse> LoadAttempt(LoadAttemptRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AttemptGrpcService: LoadAttempt ({request.Code})");
                Attempt attempt = _attemptService.GetAttempt(request.Code).Result;
                return Task.FromResult(new LoadAttemptResponse() { Attempt = attempt });
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        public override Task<LoadSymptomMeaningResponse> LoadSymptomMeaning(LoadSymptomMeaningRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AttemptGrpcService: LoadSymptomMeaning {request.Id}");
                var a = _attemptService.GetSymptomById(request.Id).Result;
                RepeatedField<SymptomMeaningEntityGrpc> b = new RepeatedField<SymptomMeaningEntityGrpc>();
                foreach (SymptomMeaning item in a.Item2)
                {
                    b.Add(item);
                }
                return Task.FromResult(new LoadSymptomMeaningResponse() 
                { 
                    Symptom = a.Item1, 
                    SymptomMeaning = { b } 
                });
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        public override Task<LoadAllSymptomMeaningResponse> LoadAllSymptomMeaning(LoadAllSymptomMeaningRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AttemptGrpcService: LoadAllSymptomMeaning");
                List<(Symptom, List<SymptomMeaning>)> list = _attemptService.GetSymptoms().Result;
                RepeatedField<LoadSymptomMeaningResponse> output = new RepeatedField<LoadSymptomMeaningResponse>();
                foreach ((Symptom, List<SymptomMeaning>) symptom in list)
                {
                    SymptomEntityGrpc a = symptom.Item1;
                    RepeatedField<SymptomMeaningEntityGrpc> b = new RepeatedField<SymptomMeaningEntityGrpc>();
                    foreach (SymptomMeaning item in symptom.Item2)
                    {
                        b.Add(item);
                    }
                    output.Add(new LoadSymptomMeaningResponse()
                    {
                        Symptom = symptom.Item1,
                        SymptomMeaning = { b }
                    });
                }
                return Task.FromResult(new LoadAllSymptomMeaningResponse() { Symptom = { output } });
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        public override Task<StartAttemptResponse> StartAttempt(StartAttemptRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AttemptGrpcService: StartAttempt {request.Message.Id}");
                Attempt attempt = _attemptService.StartAttempt(request.Message.Id).Result;
                StartAttemptResponse response = new StartAttemptResponse()
                {
                    Time = ConversionOperations.Convert(attempt.StartDateTime)
                };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        public override Task<EndAttemptResponse> EndAttempt(EndAttemptRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AttemptGrpcService: EndAttempt {request.Message.Id}");
                Attempt attempt = _attemptService.EndAttempt(request.Message.Id, request.Message.DiagnosisId, request.Message.ErrorCount).Result;
                EndAttemptResponse response = new EndAttemptResponse()
                {
                    Time = ConversionOperations.Convert(attempt.StartDateTime),
                    Grade = attempt.ErrorCount == 0,
                    ErrorCount = attempt.ErrorCount
                };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        public override Task<StartAttemptResponse> ResetAttempt(StartAttemptRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AttemptGrpcService: ResetAttempt {request.Message.Id}");
                Attempt attempt = _attemptService.ResetAttempt(request.Message.Id).Result;
                StartAttemptResponse response = new StartAttemptResponse()
                {
                    Time = ConversionOperations.Convert(DateTime.Now.AddHours(4))
                };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        public override Task<GetDiagnosesResponse> GetDiagnoses(GetDiagnosesRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: GetDiagnoses ({request.Page})");
                (List<Diagnosis> diagnoses, int currPage, int lastPage) result = _attemptService.GetDiagnoses(request.Page).Result;
                GetDiagnosesResponse response = new GetDiagnosesResponse
                {
                    Users = { ConvertFromDiagnoses(result.diagnoses) },
                    CurrentPage = result.currPage,
                    LastPage = result.lastPage
                };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        private List<DiagnosisEntityGrpc> ConvertFromDiagnoses(List<Diagnosis> list)
        {
            List<DiagnosisEntityGrpc> response = new();
            foreach (Diagnosis item in list)
            {
                response.Add(item);
            }
            return response;
        }
    }
}
