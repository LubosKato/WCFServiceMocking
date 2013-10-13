using System;
using System.Runtime.CompilerServices;
using App.Initialisation;
using App.Validation;

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

        public bool AddCustomer(string firstName, string lastName, string email, DateTime dateOfBirth, int companyId)
        {
            this.Customer = InitialiseCustomer.InitCustomer(firstName, lastName, email, dateOfBirth);

            if (this.Customer != null)
            {
                this.Customer.Company = this.CompanyRepository.GetById(companyId);
            }
            else
            {
                //TODO: add error message for customer
                return false;
            }

            if (!new CustomerValidation(this.Customer).ValidateCustomer())
            {
                return false;
            }

            switch (this.Customer.Company.Classification)
            {
                case Classification.Gold:
                    this.Customer.HasCreditLimit = false;
                    break;
                case Classification.Silver:
                    this.Customer.HasCreditLimit = true;
                    using (this.CustomerCreditService as IDisposable)
                    {
                        int creditLimit = this.CustomerCreditService.GetCreditLimit(this.Customer.FirstName, this.Customer.LastName,
                                                                               this.Customer.DateOfBirth);
                        this.Customer.CreditLimit = creditLimit * 2;
                    }
                    break;
                default:
                    this.Customer.HasCreditLimit = true;
                    using (this.CustomerCreditService as IDisposable)
                    {
                        this.Customer.CreditLimit = this.CustomerCreditService.GetCreditLimit(this.Customer.FirstName, this.Customer.LastName,
                                                                               this.Customer.DateOfBirth);
                    }
                    break;
            }

            if (this.Customer.HasCreditLimit && this.Customer.CreditLimit < 500)
            {
                //TODO: add warning message for customer
                return false;
            }

            //TODO: add information message for customer 
            this.Provider.AddCustomer(this.Customer);

            return true;
        }
    }
}
