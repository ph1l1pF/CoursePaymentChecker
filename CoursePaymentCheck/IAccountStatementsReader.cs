using System;
using System.Collections.Generic;

namespace CoursePaymentCheck
{
    public interface IAccountStatementsReader
    {
        public IAccountStatementsSource AccountStatementsSource { get; }
        public IList<AccountStatement> GetPositiveAccountStatements();
    }
}
