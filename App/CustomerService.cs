using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AppTests")]
namespace App
{
    public class CustomerService
    {
        private readonly ICompanyRepository CompanyRepository;
        private readonly ICustomerCreditService CustomerCreditService;
        private readonly ICustomerDataAccessProvider Provider;
        internal Customer Customer;

        public CustomerService(ICompanyRepository companyRepository, ICustomerCreditService customerCreditService, ICustomerDataAccessProvider provider)
        {
            this.CustomerCreditService = customerCreditService;
            this.CompanyRepository = companyRepository;
            this.Provider = provider;
        }

        public void InitCustomer(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            this.Customer = InitialiseCustomer.InitCustomer(firstName, lastName, email, dateOfBirth);
        }

        internal void InitCustomer(Customer customer)
        {
            this.Customer = customer;
        }

        public bool ValidateCustomer()
        {
            if (Customer == null)
            {
                //TODO: add error message
                return false;
            }

            if (string.IsNullOrEmpty(Customer.FirstName) || string.IsNullOrEmpty(Customer.LastName))
            {
                //TODO: add custom validation  message MissingFirstOrAndLastName
                return false;
            }

            if (!Customer.EmailAddress.Contains("@") && !Customer.EmailAddress.Contains("."))
            {
                //TODO: add custom validation  message InvalidEmailAddress
                return false;
            }

            DateTime now = DateTime.Now;
            int age = now.Year - Customer.DateOfBirth.Year;
            if (now.Month < Customer.DateOfBirth.Month ||
                (now.Month == Customer.DateOfBirth.Month && now.Day < Customer.DateOfBirth.Day)) age--;

            if (age <= 21)
            {
                //TODO: add custom validation message InvalidAge
                return false;
            }

            return true;
        }

        public bool AddCustomer(int companyId)
        {
            if (Customer != null)
            {
                Customer.Company = CompanyRepository.GetById(companyId);
            }
            else
            {
                //TODO: add error message for customer
                return false;
            }

            switch (Customer.Company.Classification)
            {
                case Classification.Gold:
                    Customer.HasCreditLimit = false;
                    break;
                case Classification.Silver:
                    Customer.HasCreditLimit = true;
                    using (CustomerCreditService as IDisposable)
                    {
                        int creditLimit = CustomerCreditService.GetCreditLimit(Customer.FirstName, Customer.LastName,
                                                                               Customer.DateOfBirth);
                        Customer.CreditLimit = creditLimit * 2;
                    }
                    break;
                default:
                    Customer.HasCreditLimit = true;
                    using (CustomerCreditService as IDisposable)
                    {
                        Customer.CreditLimit = CustomerCreditService.GetCreditLimit(Customer.FirstName, Customer.LastName,
                                                                               Customer.DateOfBirth);
                    }
                    break;
            }

            if (Customer.HasCreditLimit && Customer.CreditLimit < 500)
            {
                //TODO: add warning message for customer
                return false;
            }

            //TODO: add information message for customer 
            Provider.AddCustomer(Customer);

            return true;
        }
    }
}
