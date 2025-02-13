using Microsoft.AspNetCore.Identity;
using PriceNegotiationAPI.Model.DTO;

namespace PriceNegotiationAPI.Services
{
    public interface INegotiationService
    {
        Task<NegotiationResultDto?> StartNegotiationAsync(string productId, string userId, decimal price);
        Task<NegotiationOfferDto?> MakeNewNegotiationOfferAsync(string negotiationId, string userId, decimal price);
        Task<NegotiationResultDto?> AcceptNegotiationOfferAsync(string negotiationOfferId);
        Task<NegotiationResultDto?> RejectNegotiationOfferAsync(string negotiationOfferId);
        Task<NegotiationDto?> CancelNegotiationAsync(string negotiationId, string userId, IdentityRole identityRole);
    }
}
