using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace X_MINE.Models
{
    [Table("dokumen")]
    public class Dokumen
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Column("nama_file")]
        public string? FileName { get; set; }

        [Column("path_file")]
        public string? PathFile { get; set; }

        [Column("upload_by")]
        public string? UploadBy { get; set; }

        [Column("upload_time")]
        public DateTime? UploadTime { get; set; }

    }
}
