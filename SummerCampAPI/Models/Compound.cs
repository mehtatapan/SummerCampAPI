using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SummerCampAPI.Models
{
    [ModelMetadataType(typeof(CompoundMetaData))]
    public class Compound : Auditable
    {
        public Compound()
        {
            this.Campers = new HashSet<Camper>();
        }
        public int ID { get; set; }

        public string Name { get; set; }
        
        public ICollection<Camper> Campers { get; set; }
    }
}
