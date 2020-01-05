using System;
using System.Collections.Generic;

namespace CoursePaymentCheck
{
    public interface IAccountStatementsReader
    {
        public IList<AccountStatement> GetPositiveAccountStatements();
    }
}
