using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_Multifinance.Models.TransactionDB
{
    [Table("tr_bpkb")]
    public partial class tr_bpkb
    {
        [Key]
        [StringLength(100)]
        [Unicode(false)]
        public string agreement_number { get; set; } = null!;
        [StringLength(100)]
        [Unicode(false)]
        public string? bpkb_no { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? branch_id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? bpkb_date { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? faktur_no { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? faktur_date { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? location_id { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? police_no { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? bpkb_date_in { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? created_by { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? created_on { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? last_updated_by { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? last_updated_on { get; set; }
    }
}
