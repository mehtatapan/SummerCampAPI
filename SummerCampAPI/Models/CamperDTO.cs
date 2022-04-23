using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SummerCampAPI.Models
{
    [ModelMetadataType(typeof(CamperMetaData))]
    public class CamperDTO : IValidatableObject
    {
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        public string Gender { get; set; }

        public string EMail { get; set; }

        public string Phone { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency


        public int CompoundID { get; set; }
        public CompoundDTO Compound { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //This is an alternate way to enforce the gender value restriction without a RegEx
            string gender = "MFNTO";
            if (!gender.Contains(Gender))
            {
                yield return new ValidationResult("Gender Code must be one of M, F, N, T OR O.", new[] { "Gender" });
            }
        }
    }

}
