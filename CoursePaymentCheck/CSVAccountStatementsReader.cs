using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CoursePaymentCheck
{
    public class CSVAccountStatementsReader : IAcountStatementsReader
    {
        
        private static readonly string Date = "Buchungstag";
        private static readonly string Sender = "Beguenstigter/Zahlungspflichtiger";
        private static readonly string Subject = "Verwendungszweck";
        private static readonly string Amount = "Betrag";

        public IAccountStatementsSource AccountStatementsSource { get; }

        public CSVAccountStatementsReader(IAccountStatementsSource accountStatementsSource)
        {
            AccountStatementsSource = accountStatementsSource;
        }

        public IEnumerable<AccountStatement> GetPositiveAccountStatements()
        {
            var lines = new List<string>(File.ReadAllLines(AccountStatementsSource.ToString()));
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
            var headlineToIndex = new Dictionary<string, int>
            {
                { Date, headlines.IndexOf(Date) },
                { Sender, headlines.IndexOf(Sender) },
                { Subject, headlines.IndexOf(Subject) },
                { Amount, headlines.IndexOf(Amount) }
            };

            return headlineToIndex;
        }
    }
}
