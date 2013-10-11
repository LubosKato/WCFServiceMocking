using System;
using System.ServiceModel;
using App;

namespace AppTests
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PartialMockService : ICustomerCreditService
    {
        public virtual int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }
    }
}