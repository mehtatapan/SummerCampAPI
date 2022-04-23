using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SummerCampAPI.Models
{
    public class CamperMetaData : Auditable, IValidatableObject
    {
        [Display(Name = "Camper")]
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

        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
            get
            {
                return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6);
            }
        }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot be more than 50 characters long.")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must enter the date of birth.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "You cannot leave the GENDER blank.")]
        [RegularExpression("^\\{|M|F|N|T|O|}$", ErrorMessage = "Gender must be one of M, F, N, T OR O")]
        [StringLength(1)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }

        [Display(Name = "Emergency Phone")]
        [Required(ErrorMessage = "Emergency Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; }

        [Display(Name = "Compound")]
        [Required(ErrorMessage = "You must select the Compound.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Compound for the Camper.")]
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
        }
    }

}
