﻿namespace eCommerceApp.Application.DTOs.Cart
{
    public class GetArchieve
    {
        public string? ProductName { get; set; }
        public int QuantityOrdered { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public decimal AmountPayed { get; set; }
        public DateTime DatePurchased { get; set; }

    }
}
