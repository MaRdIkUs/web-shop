using MaxMind.GeoIP2.Responses;
using MaxMind.GeoIP2;
using System.Net;
using Diplom2.Data.Interfaces;

namespace Diplom2.Data.Analyzer
{
    public class LocalGeoService : ILocalGeoService
    {
        private readonly DatabaseReader _reader;

        public LocalGeoService(string dbPath)
        {
            _reader = new DatabaseReader(dbPath);
        }

        public (string City, string Country) GetLocation(IPAddress ipAddress)
        {
            var response = _reader.Country(ipAddress);
            return (response.Country.Name!, response.Country.Name!);
        }
    }
}
