using Microsoft.AspNetCore.Mvc;
using PoC.UnitTests.Repositories;

namespace PoC.UnitTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {
        [HttpGet("email")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);

            return Ok(user);
        }
    }
}
