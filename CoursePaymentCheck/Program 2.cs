using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursePaymentCheck;
using static CoursePaymentCheck.CoursePaymentChecker;

namespace CoursePaymentCheck
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IAccountStatementsSource accountStatementsSource = new CSVAccountStatementsSource("/Users/philipfrerk/Downloads/20191221-1888056-umsatz.CSV");
           
            IAcountStatementsReader acountStatementsReader = new CSVAccountStatementsReader(accountStatementsSource);
            var accountStatementChecker = new AccountStatementChecker();

            var startDate = new DateTime(2019, 1, 1);
            var endDate = new DateTime(2020, 1, 1);
            IEnumerable<CourseMember> members = new List<CourseMember> {
                new CourseMember("Nora", "Schmitz")
            };
            var expectedAmount = 104.0;
            CoursePaymentChecker coursePaymentChecker = new CoursePaymentChecker(acountStatementsReader, startDate, endDate,
                members, expectedAmount, accountStatementChecker);
            coursePaymentChecker.PrintInformation();


            
        }

        
    }
}
