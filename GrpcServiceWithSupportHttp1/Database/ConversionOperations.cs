using Google.Protobuf.WellKnownTypes;
using GrpcServiceWithSupportHttp1.Database.Entities;
using System.Collections.Generic;

namespace GrpcServiceWithSupportHttp1.Database
{
    public class ConversionOperations
    {
        public static Timestamp Convert(DateTime? dateTime)
        {
            if (dateTime == null || !dateTime.HasValue) return new Timestamp();
            return Timestamp.FromDateTime(DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc));
        }

        public static Timestamp Convert(DateTime dateTime)
        {
            return Timestamp.FromDateTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
        }

        public static ICollection<MedicalActionDiagnosisEntityGrpc> Convert(ICollection<MedicalActionDiagnosis> input)
        {
            ICollection<MedicalActionDiagnosisEntityGrpc> output = new List<MedicalActionDiagnosisEntityGrpc>();
            foreach (var inputItem in input)
            {
                output.Add(inputItem);
            }
            return output;
        }
    }
}
