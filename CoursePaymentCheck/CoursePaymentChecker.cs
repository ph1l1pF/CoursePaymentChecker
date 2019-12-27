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
            foreach (var (state, statements) in GetStatesOfStatements(_acountStatementsReader.GetPositiveAccountStatements()))
            {
                var text = GetTextForState(state);
                if(text != null)
                {
                    Console.WriteLine($"\n\n{text}");
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
            return state switch
            {
                AccountStatementState.LastNameCorrect => "Passender Name:",
                AccountStatementState.SubjectCorrect => "Passender Betreff:",
                AccountStatementState.AmountCorrect => "Passender Betrag:",
                AccountStatementState.NothingCorrect => null,
                AccountStatementState.EverythingCorrect => "Alles passt:",
                AccountStatementState.EverythingCorrectButDate => "Alles passt außer das Datum:",
                _ => throw new ArgumentException(),
            };
        }

        public enum AccountStatementState
        {
            EverythingCorrect=0, EverythingCorrectButDate=1, LastNameCorrect=2, AmountCorrect=3, SubjectCorrect=4,
            NothingCorrect=5
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