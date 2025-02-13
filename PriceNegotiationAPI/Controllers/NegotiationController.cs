using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PriceNegotiationAPI.Model;
using PriceNegotiationAPI.Model.DTO;
using PriceNegotiationAPI.Services;
using System.Data;

namespace PriceNegotiationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NegotiationController : ControllerBase
    {
        private readonly INegotiationService _negotiationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public NegotiationController(INegotiationService negotiationService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _negotiationService = negotiationService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("negotiation/start")]
        public async Task<ActionResult<NegotiationResultDto>> StartNegotiationAsync([FromQuery] string productId, [FromQuery] decimal price)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync("default@user.com");

            if (user == null)
                return BadRequest();

            var negotiationResultDto = await _negotiationService.StartNegotiationAsync(productId, user.Id, price);

            if (negotiationResultDto == null)
                return BadRequest();

            return Ok(negotiationResultDto);
        }

        [HttpPost("negotiationoffer/makenew")]
        public async Task<ActionResult<NegotiationOfferDto>> MakeNewNegotiationOfferAsync([FromQuery] string negotiationId, [FromQuery] decimal price)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync("default@user.com");

            if (user == null)
                return BadRequest();

            var negotiationOfferDto = await _negotiationService.MakeNewNegotiationOfferAsync(negotiationId, user.Id, price);

            if (negotiationOfferDto == null)
                return BadRequest();

            return Ok(negotiationOfferDto);
        }

        [Authorize(Roles = "Employee")]
        [HttpPatch("negotiationoffer/accept")]
        public async Task<ActionResult<NegotiationResultDto>> AcceptNegotiationOfferAsync([FromQuery] string negotiationOfferId)
        {
            var negotiationResultDto = await _negotiationService.AcceptNegotiationOfferAsync(negotiationOfferId);

            if (negotiationResultDto == null)
                return BadRequest();

            return Ok(negotiationResultDto);
        }

        [Authorize(Roles = "Employee")]
        [HttpPatch("negotiationoffer/reject")]
        public async Task<ActionResult<NegotiationResultDto>> RejectNegotiationOfferAsync([FromQuery] string negotiationOfferId)
        {
            var negotiationResultDto = await _negotiationService.RejectNegotiationOfferAsync(negotiationOfferId);

            if (negotiationResultDto == null)
                return BadRequest();

            return Ok(negotiationResultDto);
        }

        [HttpPatch("negotiation/cancel")]
        public async Task<ActionResult<NegotiationDto>> CancelNegotiationAsync([FromQuery] string negotiationId)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name!);
                if (user == null) return BadRequest();

                var role = await _roleManager.FindByNameAsync("Employee");
                if (role == null) return BadRequest();

                var negotiationDto = await _negotiationService.CancelNegotiationAsync(negotiationId, user.Id, role);
                if (negotiationDto == null) return BadRequest();

                return Ok(negotiationDto);
            }
            else
            {
                var user = await _userManager.FindByEmailAsync("default@user.com");
                if (user == null) return BadRequest();

                var role = await _roleManager.FindByNameAsync("User");
                if (role == null) return BadRequest();

                var negotiationDto = await _negotiationService.CancelNegotiationAsync(negotiationId, user.Id, role);
                if (negotiationDto == null) return BadRequest();

                return Ok(negotiationDto);
            }
        }
    }
}
