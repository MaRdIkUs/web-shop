using System.Net;

namespace Diplom2.Data.Interfaces
{
    public interface ILocalGeoService
    {
        public (string City, string Country) GetLocation(IPAddress ipAddress);
    }
}
