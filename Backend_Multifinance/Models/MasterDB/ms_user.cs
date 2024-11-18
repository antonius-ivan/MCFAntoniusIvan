using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_Multifinance.Models.MasterDB
{
    [Table("ms_user")]
    public partial class ms_user
    {
        [Key]
        public long user_id { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? user_name { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? password { get; set; }
        public bool? is_active { get; set; }
    }
}
