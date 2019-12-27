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

            var dateFitting = startDate <= accountStatement.DateTime;

            return ComputeState(propToBool, dateFitting);
        }

        private AccountStatementState ComputeState(Dictionary<string, bool> propToBool, bool dateFitting)
        {
            var state = AccountStatementState.NothingFitting;
            int numFittingProps = propToBool.Count(pair => pair.Value);

            if (numFittingProps == propToBool.Count && dateFitting) state = AccountStatementState.EverythingFitting;
            else if (numFittingProps == propToBool.Count && !dateFitting) state = AccountStatementState.EverythingButDate;
            else if (numFittingProps > 0)
            {
                if (propToBool["Amount"]) state = AccountStatementState.AmountFitting;
                if (propToBool["MemberName"]) state = AccountStatementState.LastNameFitting;
                if (propToBool["Subject"]) state = AccountStatementState.SubjectFitting;
            }

            return state;
        }

    }

}

