using Newtonsoft.Json;
using PharmacyMedicineSupplyService.Models;
using PharmacyMedicineSupplyService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PharmacyMedicineSupplyService.Provider
{
    public class PharmacySupplyProvider: IPharmacySupplyProvider
    {
        private readonly ISupplyRepo supplyRepo;
        private List<PharmacyDTO> pharmacies;
        private readonly List<PharmacyMedicineSupply> pharmacySupply=new List<PharmacyMedicineSupply>();
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PharmacySupplyProvider));

        public PharmacySupplyProvider(ISupplyRepo repo)
        {
            supplyRepo = repo;
        }
        public async Task<List<PharmacyMedicineSupply>> GetSupply(List<MedicineDemand> medicines)
        {
            pharmacies = supplyRepo.GetPharmacies();
            _log4net.Info("Pharmacies Data retieved");
            foreach (var m in medicines)
            {
                if (m.Count > 0)
                {
                    int stockCount = await GetStock(m.MedicineName);
                    if (stockCount != -1)
                    {
                        if (stockCount < m.Count)
                            m.Count = stockCount;
                        int indSupply = (m.Count) / pharmacies.Count;
                        foreach (var i in pharmacies)
                        {
                            pharmacySupply.Add(new PharmacyMedicineSupply { MedicineName = m.MedicineName, PharmacyName = i.pharmacyName, SupplyCount = indSupply });
                        }
                        if (m.Count > indSupply * pharmacies.Count)
                        {
                            pharmacySupply[pharmacySupply.Count - 1].SupplyCount += (m.Count - indSupply * pharmacies.Count);
                        }
                    }
                }
                
            }
            _log4net.Info("Medicines have been successfully supplied to the pharmacies");
            return pharmacySupply;

        }
        public async Task<int> GetStock(string medicineName)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44394")
            };
            var response = await client.GetAsync("MedicineStockInformation");
            if (!response.IsSuccessStatusCode)
            {
                _log4net.Info("No data was found in stock");
                return -1;
            }
            _log4net.Info("Retrieved data from medicine stock service");
            string stringStock = await response.Content.ReadAsStringAsync();
            var medicines = JsonConvert.DeserializeObject<List<MedicineStock>>(stringStock);
            var i = medicines.Where(x => x.Name == medicineName).FirstOrDefault();
            return i.NumberOfTabletsInStock;
        }
    }
}
