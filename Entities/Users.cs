using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bot_misis.Entities
{
    class Users : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Permissions { get; set; }
        public bool BanCond { get; set; }
    }
}
