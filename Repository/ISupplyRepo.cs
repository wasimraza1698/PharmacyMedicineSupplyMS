using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PharmacyMedicineSupplyService.Models;
namespace PharmacyMedicineSupplyService.Repository
{
    public interface ISupplyRepo
    {
        public List<PharmacyDTO> GetPharmacies();
    }
}
