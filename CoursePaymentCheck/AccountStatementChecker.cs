using System;
using System.Collections.Generic;
using System.Linq;
using static CoursePaymentCheck.CoursePaymentChecker;

namespace CoursePaymentCheck
{
    public class AccountStatementChecker
    {
        public AccountStatementState CheckStatement(AccountStatement accountStatement, IEnumerable<CourseMember> members,
            double expectedAmount, DateTime startDate)
        {
            var propToBool = new Dictionary<string, bool> {
                {"Amount", false },
                {"MemberName", false },
                {"Subject", false }
            };

            propToBool["Amount"] = Math.Abs(accountStatement.Amount - expectedAmount) <= 0;

            propToBool["MemberName"] = members.Any(member => accountStatement.SenderOrReceiver.
                Contains(member.LastName, StringComparison.OrdinalIgnoreCase));
            

            var validSubjects = new List<string> { "yoga", "kurs" };

            propToBool["Subject"] =
                     validSubjects.Any(subject => accountStatement.Subject.Contains(
                         subject, StringComparison.OrdinalIgnoreCase));

            var dateCorrect = startDate <= accountStatement.Date;

            return ComputeState(propToBool, dateCorrect);
        }

        private AccountStatementState ComputeState(Dictionary<string, bool> propToBool, bool dateFitting)
        {
            AccountStatementState state = AccountStatementState.NothingCorrect;
            int numCorrectProps = propToBool.Count(pair => pair.Value);

            if (numCorrectProps == propToBool.Count && dateFitting) state = AccountStatementState.EverythingCorrect;
            else if (numCorrectProps == propToBool.Count && !dateFitting) state = AccountStatementState.EverythingCorrectButDate;
            else if (numCorrectProps > 0)
            {
                // when multiple properties are correct, the state is overridden, which is acceptable.
                if (propToBool["Amount"]) state = AccountStatementState.AmountCorrect;
                if (propToBool["MemberName"]) state = AccountStatementState.LastNameCorrect;
                if (propToBool["Subject"]) state = AccountStatementState.SubjectCorrect;
            }

            return state;
        }

    }

}

