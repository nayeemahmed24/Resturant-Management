using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Model.Input_Model
{
    public class PhotoUpdate
    {
        public IFormFile profilePhoto { get; set; }
        public IFormFile reastaurantImage { get; set; }
    }
}
