using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SummerCampAPI.Models
{
    [ModelMetadataType(typeof(CamperMetaData))]
    public class Camper : Auditable, IValidatableObject
    {
        public int ID { get; set; }

        public string FullName
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                        (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        public string FormalName
        {
            get
            {
                return LastName + ", " + FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? "" :
                        (" " + (char?)MiddleName[0] + ".").ToUpper());
            }
        }

        public string Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int a = today.Year - DOB.Year
                    - ((today.Month < DOB.Month || (today.Month == DOB.Month && today.Day < DOB.Day) ? 1 : 0));
                return a.ToString(); /*Note: You could add .PadLeft(3) but spaces disappear in a web page. */
            }
        }

        public string PhoneFormatted
        {
            get
            {
                return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6);
            }
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        public string Gender { get; set; }

        public string EMail { get; set; }

        public string Phone { get; set; }

        public int CompoundID { get; set; }
        public Compound Compound { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //This is an alternate way to enforce the gender value restriction without a RegEx
            string gender = "MFNTO";
            if (!gender.Contains(Gender))
            {
                yield return new ValidationResult("Gender Code must be one of M, F, N, T OR O.", new[] { "Gender" });
            }
            if (int.Parse(Age) < 4)
            {
                yield return new ValidationResult("Camper must be at least 4 years old.", new[] { "DOB" });
            }
        }
    }

}
