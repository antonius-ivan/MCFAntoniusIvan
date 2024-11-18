using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend_Multifinance.Models
{
    public class StorageLocationViewModel
    {
        [Required]
        [StringLength(10, ErrorMessage = "Location ID cannot exceed 10 characters.")]
        public string location_id { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Location Name cannot exceed 100 characters.")]
        public string? location_name { get; set; }
    }
}
