﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace X_MINE.Models
{
    [Table("tbl_r_dept")]
    public class tbl_r_dept
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("dept_code")]
        public string? dept_code { get; set; }
        [Column("departemen")]
        public string? departemen { get; set; }
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
