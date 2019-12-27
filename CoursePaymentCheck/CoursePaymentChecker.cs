using System;
using System.Collections.Generic;
using System.Linq;
using CoursePaymentCheck;

namespace CoursePaymentCheck
{
    public class CoursePaymentChecker
    {
        private readonly IAcountStatementsReader _acountStatementsReader;
        private readonly AccountStatementChecker _accountStatementChecker;
        private readonly DateTime _startDate, _endDate;
        private readonly IEnumerable<CourseMember> _members;
        private readonly double _expectedAmount;


        public CoursePaymentChecker(IAcountStatementsReader acountStatementsReader, DateTime startDate,
            DateTime endDate, IEnumerable<CourseMember> members, double expectedAmount, AccountStatementChecker accountStatementChecker)
        {
            _acountStatementsReader = acountStatementsReader;
            _startDate = startDate;
            _endDate = endDate;
            _members = members;
            _expectedAmount = expectedAmount;
            _accountStatementChecker = accountStatementChecker;
        }



        public void PrintInformation()
        {
            var dictionary = GetStatesOfStatements(_acountStatementsReader.GetPositiveAccountStatements());
            
            foreach (var (state, statements) in dictionary)
            {
                var text = GetTextForState(state);
                if(text != null)
                {
                    Console.WriteLine("\n\n"+text);
                    foreach (var statement in statements) Console.WriteLine(statement);
                }
            }

        }


        private SortedDictionary<AccountStatementState, IList<AccountStatement>> GetStatesOfStatements(IEnumerable<AccountStatement> accountStatements)
        {
            var stateToStatementList = new SortedDictionary<AccountStatementState, IList<AccountStatement>>();
            foreach (var accountStatement in accountStatements)
            {
                var state = _accountStatementChecker.CheckStatement(accountStatement, _members, _expectedAmount, _startDate);
                if (stateToStatementList.ContainsKey(state)) stateToStatementList[state].Add(accountStatement);
                else stateToStatementList[state] = new List<AccountStatement> { accountStatement };
            }


            return stateToStatementList;
        }


        private string GetTextForState(AccountStatementState state)
        {
            switch (state)
            {
                case AccountStatementState.LastNameFitting: return "Passender Name";
                case AccountStatementState.SubjectFitting: return "Passender Betreff";
                case AccountStatementState.NothingFitting: return null;
                case AccountStatementState.EverythingFitting: return "Alles passt:";
                case AccountStatementState.EverythingButDate: return "Alles passt außer das Datum";
                case AccountStatementState.AmountFitting: return "Passender Betrag";
                default: throw new ArgumentException();
            }
        }

        public enum AccountStatementState
        {
            EverythingFitting=0, EverythingButDate=1, LastNameFitting=2, AmountFitting=3, SubjectFitting=4,
            NothingFitting=5
        }


        public class CourseMember
        {
            public string FirstName { get; }
            public string LastName { get; }

            public CourseMember(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }
        }
    }
    
}