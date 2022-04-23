using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SummerCampAPI.Models
{
    [ModelMetadataType(typeof(CompoundMetaData))]
    public class CompoundDTO
    {
        public CompoundDTO()
        {
            this.Campers = new HashSet<CamperDTO>();
        }
        public int ID { get; set; }

        public string Name { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency
                    
        public ICollection<CamperDTO> Campers { get; set; }
    }
}
