using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CrediBank.Controllers
{
    [ApiController]
    [Route("/")]
    public class DigitalCheckController : ControllerBase
    {
        private readonly ILogger<DigitalCheckController> _logger;
        private readonly IOptions<ApiBehaviorOptions> _apiBehavihourOptions;
        private static Random RNG = new Random();

        public DigitalCheckController(ILogger<DigitalCheckController> logger,
                                      IOptions<ApiBehaviorOptions> apiBehavihourOptions)
        {
            _logger = logger;
            _apiBehavihourOptions = apiBehavihourOptions;
        }

        [HttpGet("check/{accountId}/ammount/{ammountValue}")]
        public IActionResult GetDigitalCheck(double accountId, float ammountValue)
        {
            _logger.LogInformation($"Request Recieved for new Digital Check for account: {accountId}, with ammount: {ammountValue}");
            if (accountId.ToString().Length != 8)
            {
                ModelState.AddModelError(nameof(accountId), "accountID must be an 8 digit number");
                _logger.LogError($"accountId NOT VALID. \"{accountId}\" is not an 8 digit number");
            }
            if (ammountValue <= 0)
            {
                ModelState.AddModelError(nameof(ammountValue), "ammount must be a positive number");
                _logger.LogError($"ammount NOT VALID. \"{ammountValue}\" is not a positive number");
            }
            if (!ModelState.IsValid)
            {
                return _apiBehavihourOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
            }

            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(RNG.Next(10).ToString());
            }

            var now = DateTime.Now;

            DigitalCheck check = new DigitalCheck
            {
                CheckID = builder.ToString(),
                Date = String.Format("{0:s}", now)
            };

            _logger.LogInformation($"Operation Sucessful. Returned Digital Check {check.CheckID}, for account {accountId} with value {ammountValue}");
            return Ok(check);
        }

        [HttpGet("/")]
        public IActionResult GetRoot()
        {
            return Ok("Welcome to CrediBank REST API \n\nTo request a new Digital Check please use \".../check/{credit_account_id}/ammount/{value}\"");
        }
    }
}
