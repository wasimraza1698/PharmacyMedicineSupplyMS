using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyMedicineSupplyService.Models;
using PharmacyMedicineSupplyService.Provider;
using PharmacyMedicineSupplyService.Repository;

namespace PharmacyMedicineSupplyService.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PharmacySupplyController : ControllerBase
    {
        private readonly IPharmacySupplyProvider _provider;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PharmacySupplyController));
        public PharmacySupplyController(IPharmacySupplyProvider provider)
        {
            _provider = provider;
        }
        [HttpPost("Get")]
        public async Task<IActionResult> Get(List<MedicineDemand> m)
        {
            try
            {
                var res = await _provider.GetSupply(m);
                if(res!=null)
                {
                    _log4net.Info("Pharmacy supply details successfully retrieved and sent.");
                    return Ok(res);
                }
                _log4net.Info("No details retrieved");
                return NotFound("No such details found please try again.");
            }
            catch(Exception e)
            {
                _log4net.Error("Excpetion:" + e.Message + " has occurred while trying to retrieve supply info.");
                return StatusCode(500);
            }

            
        }
    }
}