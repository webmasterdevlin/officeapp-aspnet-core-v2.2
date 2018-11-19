using System.Threading.Tasks;
using aspnetcorebackend.Contracts;
using aspnetcorebackend.Helpers;
using aspnetcorebackend.Models.Dtos;
using aspnetcorebackend.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcorebackend.Controllers
{
    [Route("api/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repo;

        public RegisterController(IMapper mapper, IUserRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
        }
        
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try
            {
                // save 
                await _repo.CreateAsync(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}