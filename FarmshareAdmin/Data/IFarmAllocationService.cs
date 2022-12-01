using System.Security.Cryptography;

namespace FarmshareAdmin.Data
{
    public interface IFarmAllocationService
    {
        public Task<Data.Vm_Farm_Allocations> Get();
        public Task UpdateAsync(Vm_Farm_Allocations fa);
    }
}
