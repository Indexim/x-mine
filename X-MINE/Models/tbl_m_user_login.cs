using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace X_MINE.Models
{
    [Table("tbl_m_user_login")]
    public class tbl_m_user_login
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("kategori_user_id")]
        public string? kategori_user_id { get; set; }

        [Column("dept_code")]
        public string? dept_code { get; set; }

        [Column("nik")]
        public string? nik { get; set; }

        [Column("password")]
        public string? password { get; set; }

        [Column("nama")]
        public string? nama { get; set; }

        [Column("insert_by")]
        public string? insert_by { get; set; }

        [Column("ip")]
        public string? ip { get; set; }

        [Column("created_at")]
        public string? created_at { get; set; }

        [Column("updated_at")]
        public string? updated_at { get; set; }
    }
}
