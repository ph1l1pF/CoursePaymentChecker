using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CoursePaymentCheck
{
    [TestClass]
    public class CSVAccountStatementsReaderTest
    {
        private CSVAccountStatementsReader _sut;
        private readonly string _pathOfCsvFile = "file.csv";
        private readonly string _firstLineOfFile = $"\"{CSVAccountStatementsReader.Date}\";\"{CSVAccountStatementsReader.Sender}\";" +
            $"\"{CSVAccountStatementsReader.Subject}\";\"{CSVAccountStatementsReader.Amount}\"";

        private readonly string _sender = "Max Muster";
        private readonly string _subject = "SomeSubject";
        private readonly string _amountString = "100,02";


        [TestInitialize]
        public void Init()
        {
            var accountStatementsSource =  Substitute.For<IAccountStatementsSource>();
            accountStatementsSource.ToString().Returns(_pathOfCsvFile);
            _sut = new CSVAccountStatementsReader(accountStatementsSource);
            File.AppendAllText(_pathOfCsvFile, _firstLineOfFile);
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(_pathOfCsvFile);
        }

        [TestMethod]
        public void TestGetPositiveAccountStatements_OnePositiveStatement()
        {
            var statementLine = $"\"12.01.2019\";\"{_sender}\";\"{_subject}\";\"{_amountString}\"";
            File.AppendAllText(_pathOfCsvFile, "\n" + statementLine);

            var resultStatements = _sut.GetPositiveAccountStatements();
            resultStatements.Count.Should().Be(1);

            var actualStatement = resultStatements[0];

            actualStatement.DateTime.Should().Be(new DateTime(2019, 1, 12));
            actualStatement.SenderOrReceiver.Should().Be(_sender);
            actualStatement.Subject.Should().Be(_subject);
            actualStatement.Amount.Should().Be(100.02);
        }

        [TestMethod]
        public void TestGetPositiveAccountStatements_OneNonPositiveStatement()
        {
            var negativeAmount = "-100,30";
            var statementLine = $"\"12.01.2019\";\"{_sender}\";\"{_subject}\";\"{negativeAmount}\"";
            File.AppendAllText(_pathOfCsvFile, "\n" + statementLine);

            var resultStatements = _sut.GetPositiveAccountStatements();
            resultStatements.Count.Should().Be(0);
        }
    }
}
