﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interact.Instance.Data.Postgresql.InteractDomain
{
    [Table("consumer_server_status")]
    public partial class ConsumerServerStatus
    {
        public ConsumerServerStatus()
        {
            CloudInstance = new HashSet<CloudInstance>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }

        [InverseProperty("ConsumerServerStatus")]
        public ICollection<CloudInstance> CloudInstance { get; set; }
    }
}
