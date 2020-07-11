using System.Threading.Tasks;

namespace Bootable.ProjectSystem
{
    public interface IBootableProperties
    {
        Task<string> GetBinFileFullPathAsync();
        Task<string> GetIsoFileFullPathAsync();
    }
}
