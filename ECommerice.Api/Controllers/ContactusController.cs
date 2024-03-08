using AutoMapper;
using ECommerice.Api.DTOs;
using ECommerice.Api.Helpers;
using ECommerice.Api.Helpers.Extensions;
using ECommerice.Core.DTOs;
using ECommerice.Core.Entities;
using ECommerice.Core.Entities.OrderAggregate;
using ECommerice.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactusController : BaseApiController
    {
        private readonly IContactusRepo _contactRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public ContactusController(IMapper mapper,
            IContactusRepo contactRepo, UserManager<AppUser> userManager)
        {
            _contactRepo = contactRepo;
            _mapper = mapper;
            _userManager = userManager;
        }


        [HttpPost("send")]
        public async Task<ActionResult<Order>> Send(Contactus contactus)
        {
            var email = HttpContext.User?.RetrieveEmailFromClaimPrincipal();
            var user = await _userManager.FindByEmailAsync(email);
            contactus.UserId = user.Id;

            var result = await _contactRepo.Send(contactus);
            if(result)
                return Ok();

            return BadRequest();
        }

        [HttpPost("replay")]
        public async Task<ActionResult<Order>> Replay([FromQuery]string replay)
        {
            var email = HttpContext.User?.RetrieveEmailFromClaimPrincipal();
            var user = await _userManager.FindByEmailAsync(email);
            
            var result = await _contactRepo.Replay(user.Id, replay);
            if (result)
                return Ok();

            return BadRequest();
        }

        [HttpGet("getQuestionWithReplayes")]
        public async Task<IActionResult> GetQuestionWithReplayes()
        {
            var email = HttpContext.User?.RetrieveEmailFromClaimPrincipal();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return BadRequest();

            return Ok(await _contactRepo.GetQuestionWithReplayes(user.Id));
        }

        #region Admin only ======================================================

        [HttpGet("getQuestions")]
        public async Task<IActionResult> GetQuestions()
        {
            return Ok(await _contactRepo.GetQuestions());
        }

        [HttpPost("sendAnswer")]
        public async Task<IActionResult> SendAnswer(ContactusReplayDTO contactusReplay)
        {
            return Ok(await _contactRepo.SendAnswer(contactusReplay));
        }
        #endregion

    }
}
