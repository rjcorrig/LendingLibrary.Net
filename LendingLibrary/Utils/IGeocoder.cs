using System;
using System.Data.Entity.Spatial;
using System.Threading.Tasks;

namespace LendingLibrary.Utils
{
    public interface IGeocoder
    {
        Task<DbGeography> GeocodeAsync(string address);
    }
}
