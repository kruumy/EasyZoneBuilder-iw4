using System.Threading.Tasks;

namespace EasyZoneBuilder.Core.Interfaces
{
    public interface IASync
    {
        Task Push();
        Task Pull();
    }
}
