using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CoursePaymentCheck.CoursePaymentChecker;

namespace CoursePaymentCheck
{
    [TestClass]
    public class AccountStatementCheckerTest
    {
        private AccountStatementChecker _sut;
        private List<CourseMember> _members;
        private double _amount;
        private string _subject;
        private DateTime _startDate;

        [TestInitialize]
        public void Init()
        {
            _sut = new AccountStatementChecker();
            _members = new List<CourseMember> { new CourseMember("Rita", "Peters") };
            _amount = 100;
            _subject = "Yoga";
            _startDate = new DateTime(2020, 1, 1);
        }

        [TestMethod]
        public void Test_CheckStatement_EverythingCorrect()
        {
            var accountStatement = new AccountStatement(_startDate, _members[0].LastName, _subject, _amount);

            _sut.CheckStatement(accountStatement, _members, _amount, _startDate).
                Should().Be(AccountStatementState.EverythingCorrect);
        }

        [TestMethod]
        public void Test_CheckStatement_EverythingCorrectButDate()
        {
            var falseDate = _startDate - TimeSpan.FromDays(1);
            var accountStatement = new AccountStatement(falseDate, _members[0].LastName, _subject, _amount);

            _sut.CheckStatement(accountStatement, _members, _amount, _startDate).
                Should().Be(AccountStatementState.EverythingCorrectButDate);
        }

        [TestMethod]
        public void Test_CheckStatement_FalseAmount()
        {
            var falseAmount = _amount + 1;
            var accountStatement = new AccountStatement(_startDate, _members[0].LastName, _subject, falseAmount);

            var result = _sut.CheckStatement(accountStatement, _members, _amount, _startDate);
            result.Should().NotBe(AccountStatementState.NothingCorrect);
            result.Should().NotBe(AccountStatementState.EverythingCorrect);
            result.Should().NotBe(AccountStatementState.EverythingCorrectButDate);
        }

        [TestMethod]
        public void Test_CheckStatement_FalseSubject()
        {
            var falseSubject = "bla";
            var accountStatement = new AccountStatement(_startDate, _members[0].LastName, falseSubject, _amount);

            var result = _sut.CheckStatement(accountStatement, _members, _amount, _startDate);
            result.Should().NotBe(AccountStatementState.NothingCorrect);
            result.Should().NotBe(AccountStatementState.EverythingCorrect);
            result.Should().NotBe(AccountStatementState.EverythingCorrectButDate);
        }
    }
}
