using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CoursePaymentCheck
{
    class FinTsAccountStatementsReader : IAccountStatementsReader
    {
        public IAccountStatementsSource AccountStatementsSource => throw new NotImplementedException();

        private readonly string _accountNumber;
        private readonly DateTime _startDate, _endDate;
        private readonly string _httpsEndpoint;
        private readonly string _bankNumber;
        private readonly string _pin;

        private static readonly string FileCsvPath = @"C:\Users\phili\Desktop\file1.csv";
        private static readonly string FilePythonPath = @"C:\Users\phili\Desktop\gettransactions.py";
        public static readonly string Date = "Buchungstag";
        public static readonly string Sender = "Beguenstigter/Zahlungspflichtiger";
        public static readonly string Subject = "Verwendungszweck";
        public static readonly string Amount = "Betrag";

        public FinTsAccountStatementsReader(string accountNumber, DateTime startDate, DateTime endDate, string httpsEndpoint, string bankNumber, string pin)
        {
            _accountNumber = accountNumber;
            _startDate = startDate;
            _endDate = endDate;
            _httpsEndpoint = httpsEndpoint;
            _bankNumber = bankNumber;
            _pin = pin;
        }

        public IList<AccountStatement> GetPositiveAccountStatements()
        {
            File.Delete(FileCsvPath);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;//Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $@"/C python3 {FilePythonPath} {_bankNumber} {_accountNumber} {_accountNumber} {_pin}" +
                $" {_startDate.ToString("yyyy-MM-dd")} {_endDate.ToString("yyyy-MM-dd")} {_httpsEndpoint} {FileCsvPath}";

            
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            return ReadStatementsFromFile();
        }

        private IList<AccountStatement> ReadStatementsFromFile()
        {
            var lines = new List<string>(File.ReadAllLines(FileCsvPath));
            
            var headLineToIndex = GetIndexesOfHeadlines(lines[0]);
            lines.RemoveAt(0);
            var positiveAccountStatements = new List<AccountStatement>();

            foreach (var line in lines)
            {
                string[] columnValues = line.Split(";");
                var dateTime = DateTime.Parse(columnValues[headLineToIndex[Date]]);

                var amountStringWithPoint = columnValues[headLineToIndex[Amount]].Replace(",", ".");
                var amount = double.Parse(amountStringWithPoint, NumberStyles.Any, CultureInfo.InvariantCulture);

                var accStatement = new AccountStatement(dateTime,
                    columnValues[headLineToIndex[Sender]],
                    columnValues[headLineToIndex[Subject]],
                    amount);
                if (accStatement.IsPositive) positiveAccountStatements.Add(accStatement);

            }

            return positiveAccountStatements;
        }

       

        private IDictionary<string, int> GetIndexesOfHeadlines(string firstLineOfFile)
        {
            var headlines = new List<string>(firstLineOfFile.Split(";"));
            return new Dictionary<string, int>
            {
                { Date, headlines.IndexOf(Date) },
                { Sender, headlines.IndexOf(Sender) },
                { Subject, headlines.IndexOf(Subject) },
                { Amount, headlines.IndexOf(Amount) }
            };
        }
    }
}
