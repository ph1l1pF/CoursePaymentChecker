using System;
using System.Collections.Generic;

namespace CoursePaymentCheck
{
    public interface IAcountStatementsReader
    {
        public IAccountStatementsSource AccountStatementsSource { get; }
        public IList<AccountStatement> GetPositiveAccountStatements();
    }
}
