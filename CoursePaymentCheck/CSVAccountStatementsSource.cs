using System;
namespace CoursePaymentCheck
{
    public class CSVAccountStatementsSource : IAccountStatementsSource
    {
        public string SourceFilePath { get; set; }

        public CSVAccountStatementsSource(string sourceFilePath)
        {
            SourceFilePath = sourceFilePath;
        }

        public override string ToString()
        {
            return SourceFilePath;
        }
    }
}
