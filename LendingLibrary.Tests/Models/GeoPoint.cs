using NUnit.Framework;
using System;
using LendingLibrary.Models;

namespace LendingLibrary.Tests.Models
{
    [TestFixture()]
    public class GeoPointTests
    {
        [Test()]
        public void Constructor_returns_GeoPoint_if_valid()
        {
            var lat = 41.0;
            var lng = -81.5;
            var point = new GeoPoint(lat, lng);
            Assert.AreEqual(lat, point.Latitude);
            Assert.AreEqual(lng, point.Longitude);
        }

        [Test()]
        public void Constructor_throws_ArgumentOutOfRangeException_if_latitude_out_of_range()
        {
            var lat = 100.0;
            var lng = -81.5;

            var argException = Assert.Throws<ArgumentOutOfRangeException>(() => new GeoPoint(lat, lng));
        }

        [Test()]
        public void Constructor_throws_if_longitude_out_of_range()
        {
            var lat = 41.0;
            var lng = -181.5;

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new GeoPoint(lat, lng));
        }

    }
}
