
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceNegotiationAPI.Database;
using PriceNegotiationAPI.Enums;
using PriceNegotiationAPI.Model;
using PriceNegotiationAPI.Model.DTO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace PriceNegotiationAPI.Services
{
    public class NegotiationService : INegotiationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public NegotiationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<NegotiationResultDto?> StartNegotiationAsync(string productId, string userId, decimal price)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.ProductStatus == ProductStatus.Available);

            if (product == null)
                return null;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var negotiation = await _context.Negotiations.FirstOrDefaultAsync(n => n.ProductId == productId && n.UserId == userId && n.ValidUntil > DateTime.Now 
            && (n.NegotiationStatus == NegotiationStatus.Ongoing || n.NegotiationStatus == NegotiationStatus.Waiting_For_User));

            if (negotiation != null)
                return null;

            var newNegotiation = new Negotiation
            {
                UserId = userId,
                CreatedBy = user,
                ProductId = productId,
                Product = product,
                NegotiationStatus = NegotiationStatus.Ongoing,
                RejectsCounter = 0,
                ValidUntil = DateTime.Now.AddDays(7)
            };

            var newNegotiationOffer = new NegotiationOffer
            {
                NegotiationId = newNegotiation.Id,
                Negotiation = newNegotiation,
                Price = price,
                CreatedAt = DateTime.Now
            };

            newNegotiation.NegotiationOffers.Add(newNegotiationOffer);
            product.Negotiations.Add(newNegotiation);

            await _context.Negotiations.AddAsync(newNegotiation);
            await _context.NegotiationOffers.AddAsync(newNegotiationOffer);
            await _context.SaveChangesAsync();

            return new NegotiationResultDto
            {
                Negotiation = _mapper.Map<NegotiationDto>(newNegotiation),
                NegotiationOffer = _mapper.Map<NegotiationOfferDto>(newNegotiationOffer)
            };
        }

        public async Task<NegotiationOfferDto?> MakeNewNegotiationOfferAsync(string negotiationId, string userId, decimal price)
        {
            var user = _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var negotiation = await _context.Negotiations.FirstOrDefaultAsync(n => n.Id == negotiationId && n.UserId == userId && n.ValidUntil > DateTime.Now
            && n.NegotiationStatus == NegotiationStatus.Waiting_For_User);

            if (negotiation == null) 
                return null;

            var negotiationOffer = await _context.NegotiationOffers.OrderByDescending(n => n.CreatedAt).FirstOrDefaultAsync();

            if (negotiationOffer == null)
                return null;

            if (negotiationOffer.Price >= price) 
                return null;


            var newDateTime = DateTime.Now;
            negotiation.NegotiationStatus = NegotiationStatus.Ongoing;
            negotiation.ValidUntil = newDateTime.AddDays(7);

            var newNegotiationOffer = new NegotiationOffer
            {
                NegotiationId = negotiationId,
                Negotiation = negotiation,
                CreatedAt = newDateTime,
                Price = price
            };

            await _context.NegotiationOffers.AddAsync(newNegotiationOffer);
            await _context.SaveChangesAsync();

            return _mapper.Map<NegotiationOfferDto>(newNegotiationOffer);
        }

        public async Task<NegotiationResultDto?> AcceptNegotiationOfferAsync(string negotiationOfferId)
        {
            var negotiationOffer = await _context.NegotiationOffers.FindAsync(negotiationOfferId);

            if (negotiationOffer == null) 
                return null;

            var negotiation = await _context.Negotiations.FirstOrDefaultAsync(n => n.Id == negotiationOffer.NegotiationId && n.ValidUntil > DateTime.Now && n.NegotiationStatus == NegotiationStatus.Ongoing);

            if (negotiation == null) 
                return null;

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == negotiation.ProductId && p.ProductStatus == ProductStatus.Available);

            if (product == null)
                return null;

            var otherNegotiationsToCancel = _context.Negotiations.Where(n => n.ProductId == product.Id && (n.NegotiationStatus == NegotiationStatus.Ongoing || n.NegotiationStatus == NegotiationStatus.Waiting_For_User));

            foreach(var negotiationToCancel in otherNegotiationsToCancel)
            {
                negotiationToCancel.NegotiationStatus = NegotiationStatus.Canceled;
            }

            product.ProductStatus = ProductStatus.Sold;
            negotiation.NegotiationStatus = NegotiationStatus.Accepted;

            await _context.SaveChangesAsync();

            return new NegotiationResultDto
            {
                Negotiation = _mapper.Map<NegotiationDto>(negotiation),
                NegotiationOffer = _mapper.Map<NegotiationOfferDto>(negotiationOffer)
            };
        }

        public async Task<NegotiationResultDto?> RejectNegotiationOfferAsync(string negotiationOfferId)
        {
            var negotiationOffer = await _context.NegotiationOffers.FindAsync(negotiationOfferId);

            if (negotiationOffer == null)
                return null;

            var negotiation = await _context.Negotiations.FirstOrDefaultAsync(n => n.Id == negotiationOffer.NegotiationId && n.ValidUntil > DateTime.Now && n.NegotiationStatus == NegotiationStatus.Ongoing);

            if (negotiation == null)
                return null;

            if (negotiation.RejectsCounter < 3)
            {
                negotiation.RejectsCounter++;
                negotiation.NegotiationStatus = NegotiationStatus.Waiting_For_User;
            }
            else
            {
                negotiation.RejectsCounter++;
                negotiation.NegotiationStatus = NegotiationStatus.Canceled;
            }

            await _context.SaveChangesAsync();

            return new NegotiationResultDto
            {
                Negotiation = _mapper.Map<NegotiationDto>(negotiation),
                NegotiationOffer = _mapper.Map<NegotiationOfferDto>(negotiationOffer)
            };
        }

        public async Task<NegotiationDto?> CancelNegotiationAsync(string negotiationId, string userId, IdentityRole identityRole)
        {
            var negotiation = await _context.Negotiations.FindAsync(negotiationId);
            var roleName = identityRole.Name;

            if (negotiation == null)
                return null;

            if (String.IsNullOrWhiteSpace(roleName))
                return null;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) 
                return null;

            if (roleName == "Employee" || (roleName == "User" && negotiation.UserId == userId))
            {
                negotiation.NegotiationStatus = NegotiationStatus.Canceled;
                await _context.SaveChangesAsync();
                return _mapper.Map<NegotiationDto>(negotiation);
            }

            return null;
        }
    }
}