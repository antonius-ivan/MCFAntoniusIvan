using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_Multifinance.Models.MasterDB
{
    [Table("ms_storage_location")]
    public partial class ms_storage_location
    {
        [Key]
        [StringLength(10)]
        [Unicode(false)]
        public string location_id { get; set; } = null!;
        [StringLength(100)]
        [Unicode(false)]
        public string? location_name { get; set; }
    }
}
