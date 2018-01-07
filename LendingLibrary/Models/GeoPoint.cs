/*
 * LendingLibrary - An online private bookshelf catalog and sharing application
 * Copyright (C) 2017 Robert Corrigan
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel.DataAnnotations;

namespace LendingLibrary.Models
{
    public class GeoPoint
    {
        public const double LATITUDE_MAX = 90.0;
        public const double LATITUDE_MIN = -90.0;

        public const double LONGITUDE_MAX = 180.0;
        public const double LONGITUDE_MIN = -180.0;

        [Range(LATITUDE_MIN, LATITUDE_MAX)]
        public double Latitude { get; private set; }
        [Range(LONGITUDE_MIN, LONGITUDE_MAX)]
        public double Longitude { get; private set; }

        public GeoPoint(double Latitude, double Longitude)
        {
            if (Latitude < LATITUDE_MIN || Latitude > LATITUDE_MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(Latitude), Latitude, 
                                                      $"{nameof(Latitude)} must be between {LATITUDE_MIN} and ${LATITUDE_MAX}");
            }

            if (Longitude < LONGITUDE_MIN || Longitude > LONGITUDE_MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(Longitude), Longitude,
                                                      $"{nameof(Longitude)} must be between {LONGITUDE_MIN} and {LONGITUDE_MAX}");
            }

            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }
    }
}
