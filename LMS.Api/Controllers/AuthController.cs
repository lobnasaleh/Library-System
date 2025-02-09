using AutoMapper;
using LMS.Api.DTOs;
using LMS.Core.Contracts;
using LMS.Core.Interfaces;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper mapper ;
        private readonly IFileService fileService;

        public AuthController(IAuthService _authService, IMapper mapper, IFileService fileService)
        {
            this._authService = _authService;
            this.mapper = mapper;
            this.fileService = fileService;
        }
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task <ActionResult<AuthResponse>> login(LoginRequestDTO loginRequestDTO)
        {

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            //map loginrequestdto to loginrequest
           var req= mapper.Map<LoginRequest>(loginRequestDTO);
          var result=  await _authService.Login(req);

            if (!result.isAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);

        }
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task< ActionResult<AuthResponse>> Register([FromForm]RegisterRequestDTO registerRequestDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //map registerRequestDto to REgisterRequest
            var req = mapper.Map<RegisterRequest>(registerRequestDTO);

            var imageUrl = fileService.SaveImage(registerRequestDTO.Image, "Users");
            if (imageUrl.Item1 == 1)
            {
                req.Image = imageUrl.Item2; // getting image url
            }

            var result = await _authService.Register(req);

            if (!result.isAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
