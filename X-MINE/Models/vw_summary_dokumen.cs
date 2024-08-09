using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace X_MINE.Models
{
    [Table("vw_summary_dokumen")]
    public class vw_summary_dokumen
    {
        [Column("status")]
        public string? status { get; set; }

        [Column("jumlah_file")]
        public long? jumlah_file { get; set; } 
    }
}
