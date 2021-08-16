using System.Threading.Tasks;

namespace StreamBadger.Shared.Clients
{
    public interface IServerClient
    {
        Task PlaySound(string name);
        Task ShowImage(string name);
    }
}