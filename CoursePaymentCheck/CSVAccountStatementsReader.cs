using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CoursePaymentCheck
{
    public class CSVAccountStatementsReader : IAccountStatementsReader
    {
        
        public static readonly string Date = "Buchungstag";
        public static readonly string Sender = "Beguenstigter/Zahlungspflichtiger";
        public static readonly string Subject = "Verwendungszweck";
        public static readonly string Amount = "Betrag";

        private readonly string _accountStatementSource;
        

        public CSVAccountStatementsReader(string accountStatementsSource)
        {
            _accountStatementSource = accountStatementsSource;
        }

        public IList<AccountStatement> GetPositiveAccountStatements()
        {
            var lines = new List<string>(File.ReadAllLines(_accountStatementSource));
            lines = RemoveQuotes(lines);
            var headLineToIndex = GetIndexesOfHeadlines(lines[0]);
            lines.RemoveAt(0);
            var positiveAccountStatements = new List<AccountStatement>();
            
            foreach(var line in lines)
            {
                string[] columnValues = line.Split(";");
                var dateTime = DateTime.Parse(columnValues[headLineToIndex[Date]],
                    new CultureInfo("de-DE"), DateTimeStyles.NoCurrentDateDefault);

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

        private List<string> RemoveQuotes(List<string> lines)
        {
            var newList = new List<string>();
            foreach(var line in lines)
            {
                newList.Add(line.Replace("\"", ""));
            }
            return newList;
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
