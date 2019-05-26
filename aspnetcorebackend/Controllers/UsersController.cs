using System.Threading.Tasks;
using aspnetcorebackend.Contracts;
using aspnetcorebackend.Helpers;
using aspnetcorebackend.Models.Dtos;
using aspnetcorebackend.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcorebackend.Controllers
{
    [Produces("application/json")]
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            
            try
            {
                // save 
                await _repo.CreateAsync(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}