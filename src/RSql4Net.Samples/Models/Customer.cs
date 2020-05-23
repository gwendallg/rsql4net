using System;

namespace RSql4Net.Samples.Models
{
    public class Customer
    {
        public bool? Active { get; set; }

        public Address Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Username { get; set; }

        public string Website { get; set; }

        public double Debit { get; set; }

        public double Credit { get; set; }
    }
}
