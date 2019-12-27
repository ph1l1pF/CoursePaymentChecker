using System;
namespace CoursePaymentCheck
{
    public class AccountStatement
    {
        public DateTime DateTime { get; }
        public string SenderOrReceiver { get; }
        public string Subject { get; }
        public double Amount { get; }

        public bool IsPositive { get { return Amount > 0; } }

        public AccountStatement(DateTime dateTime, string senderOrReceiver, string subject, double amount)
        {
            DateTime = dateTime;
            SenderOrReceiver = senderOrReceiver;
            Subject = subject;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{DateTime.ToShortDateString()}, \"{SenderOrReceiver.Trim()}\", \"{Subject.Trim()}\", {Amount} EUR";
        }

    }
}
