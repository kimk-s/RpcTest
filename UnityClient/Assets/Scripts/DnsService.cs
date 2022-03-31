using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public static class DnsService
{
    public static string LocalIPAddress { get; private set; }

    public static async Task InitializeAsync()
    {
        LocalIPAddress = await GetLocalIPAddress().ConfigureAwait(false);
    }

    private static async Task<string> GetLocalIPAddress()
    {
        var host = await Dns.GetHostEntryAsync(Dns.GetHostName()).ConfigureAwait(false);

        return string.Join(';', host.AddressList
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
            .Select(x => x.ToString()));
    }
}
