using System;
using System.Collections.Generic;
using System.Linq;
using Aldan.Core;
using Aldan.Core.Data;
using Aldan.Core.Domain.Customers;
using Aldan.Services.Events;

namespace Aldan.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IEventPublisher _eventPublisher;

        public CustomerService(
            IRepository<Customer> customerRepository, IEventPublisher eventPublisher)
        {
            _customerRepository = customerRepository;
            _eventPublisher = eventPublisher;
        }

        public IPagedList<Customer> GetAllCustomers(DateTime? createdFromUtc = null, DateTime? createdToUtc = null, Role[] customerRoles = null,
            string email = null, string ipAddress = null, int pageIndex = 0, int pageSize = Int32.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _customerRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
            
            query = query.Where(c => !c.Deleted);

            if (customerRoles != null && customerRoles.Length > 0)
                query = query.Where(z => customerRoles.Contains(z.Role));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            
            //search by IpAddress
            if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
            {
                query = query.Where(w => w.LastIpAddress == ipAddress);
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var customers = new PagedList<Customer>(query, pageIndex, pageSize, getOnlyTotalCount);
            return customers;
        }

        public IPagedList<Customer> GetOnlineCustomers(DateTime lastActivityFromUtc, Role[] customerRoles, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var query = _customerRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);
            if (customerRoles != null && customerRoles.Length > 0)
                query = query.Where(c => customerRoles.Contains(c.Role));

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var customers = new PagedList<Customer>(query, pageIndex, pageSize);
            return customers;
        }

        public void DeleteCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            customer.Deleted = true;

            UpdateCustomer(customer);

            //event notification
            _eventPublisher.EntityDeleted(customer);
        }

        public Customer GetCustomerById(int customerId)
        {
            if (customerId == 0)
                return null;

            return _customerRepository.GetById(customerId);
        }

        public IList<Customer> GetCustomersByIds(int[] customerIds)
        {
            if (customerIds == null || customerIds.Length == 0)
                return new List<Customer>();

            var query = from c in _customerRepository.Table
                where customerIds.Contains(c.Id) && !c.Deleted
                select c;
            var customers = query.ToList();
            //sort by passed identifiers
            var sortedCustomers = new List<Customer>();
            foreach (var id in customerIds)
            {
                var customer = customers.Find(x => x.Id == id);
                if (customer != null)
                    sortedCustomers.Add(customer);
            }

            return sortedCustomers;
        }

        public Customer GetCustomerByGuid(Guid customerGuid)
        {
            if (customerGuid == Guid.Empty)
                return null;

            var query = from c in _customerRepository.Table
                where c.CustomerGuid == customerGuid
                orderby c.Id
                select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        public Customer GetCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _customerRepository.Table
                orderby c.Id
                where c.Email == email
                select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        public void InsertCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _customerRepository.Insert(customer);

            //event notification
            _eventPublisher.EntityInserted(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _customerRepository.Update(customer);

            //event notification
            _eventPublisher.EntityUpdated(customer);
        }
    }
}