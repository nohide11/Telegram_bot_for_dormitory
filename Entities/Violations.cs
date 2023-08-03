using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace bot_misis.Entities
{
    class Violations : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string User { get; set; }
        public string Corpus { get; set; }
        public int Room { get; set; }
        public string Type { get; set; }
        public DateTime Data { get; set; }

        public override string ToString()
        {
            return User + " " + Corpus + " " + Room + " " + Type + " " + Data;
        }
    }
}
