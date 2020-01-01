using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace CoursePaymentCheck
{
    class FinTsAccountStatementsReader : IAccountStatementsReader
    {
     
        private readonly string _accountNumber;
        private readonly DateTime _startDate, _endDate;
        private readonly string _httpsEndpoint;
        private readonly string _bankNumber;
        private readonly string _pin;
        private readonly OS _os;
        private readonly string _fileCsvPath;
        private readonly string _filePythonPath;

        public static readonly string Date = "Buchungstag";
        public static readonly string Sender = "Beguenstigter/Zahlungspflichtiger";
        public static readonly string Subject = "Verwendungszweck";
        public static readonly string Amount = "Betrag";

        public IAccountStatementsSource AccountStatementsSource => throw new NotImplementedException();

        public FinTsAccountStatementsReader(string accountNumber, DateTime startDate, DateTime endDate,
            string httpsEndpoint, string bankNumber, string pin, OS os)
        {
            _accountNumber = accountNumber;
            _startDate = startDate;
            _endDate = endDate;
            _httpsEndpoint = httpsEndpoint;
            _bankNumber = bankNumber;
            _pin = pin;
            _os = os;

            _filePythonPath = Path.Combine(GetPythonDirectory(), "gettransactions.py");
            _fileCsvPath = Path.Combine(GetPythonDirectory(), "file1.csv"); 
        }

        private static string GetPythonDirectory()
        {
            var workingDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
            workingDirectory = Directory.GetParent(workingDirectory.FullName);
            workingDirectory = Directory.GetParent(workingDirectory.FullName);
            return Path.Combine(workingDirectory.FullName, "Python");
        }

        public IList<AccountStatement> GetPositiveAccountStatements()
        {
            File.Delete(_fileCsvPath);
            string command = $"python3 {_filePythonPath} {_bankNumber} {_accountNumber} {_accountNumber} {_pin}" +
                    $" {_startDate.ToString("yyyy-MM-dd")} {_endDate.ToString("yyyy-MM-dd")} {_httpsEndpoint} {_fileCsvPath}";
            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;


            if (_os == OS.Windows)
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $@"/C {command}";
            }
            else if(_os == OS.Mac)
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = "-c \" " + command + " \"";
            }


            process.Start();
            process.WaitForExit();

            return ReadStatementsFromFile();
        }

        private IList<AccountStatement> ReadStatementsFromFile()
        {
            var lines = new List<string>(File.ReadAllLines(_fileCsvPath));
            
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

        public enum OS { Windows, Mac}
    }
}
