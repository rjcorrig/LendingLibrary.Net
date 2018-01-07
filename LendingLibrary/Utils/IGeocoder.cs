using System;
using System.Threading.Tasks;
using LendingLibrary.Models;

namespace LendingLibrary.Utils
{
    public interface IGeocoder
    {
        Task<GeoPoint> GeocodeAsync(string address);
    }
}
