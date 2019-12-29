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
            
            IAccountStatementsReader acountStatementsReader = new FinTsAccountStatementsReader("KontoNummer", new DateTime(2019, 12, 10), new DateTime(2019, 12, 25),
                "https://banking-wl1.s-fints-pt-wl.de/fints30", "BLZ", "PIJ");
            var accountStatementChecker = new AccountStatementChecker();

            var startDate = new DateTime(2019, 12, 1);
            var endDate = new DateTime(2019, 12, 25);
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
