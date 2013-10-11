namespace App
{
    public class CustomerDataAccessProvider : ICustomerDataAccessProvider
    {
         public void AddCustomer(Customer customer)
         {
             CustomerDataAccess.AddCustomer(customer);
         }
    }
}