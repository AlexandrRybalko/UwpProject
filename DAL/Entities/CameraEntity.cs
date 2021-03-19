﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class CameraEntity
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ImageType { get; set; }

        public ICollection<CamerasCategories> CamerasCategories { get; set; }
    }
}