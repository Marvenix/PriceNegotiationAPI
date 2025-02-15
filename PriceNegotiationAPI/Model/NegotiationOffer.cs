﻿using PriceNegotiationAPI.Enums;

namespace PriceNegotiationAPI.Model
{
    public class NegotiationOffer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string NegotiationId { get; set; }
        public required Negotiation Negotiation { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
