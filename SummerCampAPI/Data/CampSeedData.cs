using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummerCampAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SummerCampAPI.Data
{
    public static class CampSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new CampWebApiContext(
                serviceProvider.GetRequiredService<DbContextOptions<CampWebApiContext>>()))
            {
                //Create sample data with some random values
                Random random = new Random();

                string[] kidNames = new string[] { "Woodstock", "Sally", "Violet", "Charlie", "Lucy", "Linus", "Franklin", "Marcie", "Schroeder", "Fred", "Barney", "Wilma", "Betty", "David" };
                string[] lastNames = new string[] { "Jones", "Bloggs", "Flintstone", "Rubble", "Brown", "Smith", "Daniel", "Hightower", "Kingfisher", "Prometheus", "Broomspun", "Shooter", "Chuckles" };
                string[] compounds = new string[] { "Nestlings", "Fledglings", "Sharpies", "Triassic", "Jurassic", "Cretaceous" };
                string[] genders = new string[] { "M", "F", "N", "T", "O" };

                //Compounds
                if (!context.Compounds.Any())
                {
                    //loop through the array of Compound names
                    foreach (string c in compounds)
                    {
                        Compound compound = new Compound()
                        {
                            Name = c
                        };
                        context.Compounds.Add(compound);
                    }
                    context.SaveChanges();
                }

                //Create collections of the primary keys
                int[] compoundIDs = context.Compounds.Select(a => a.ID).ToArray();
                int compoundIDCount = compoundIDs.Count();
                int genderCount = genders.Count();

                //Campers
                if (!context.Campers.Any())
                {
                    // Start birthdate for randomly produced campers 
                    // We will subtract a random number of days from today
                    DateTime startDOB = DateTime.Today;

                    List<Camper> campers = new List<Camper>();
                    foreach (string lastName in lastNames)
                    {
                        foreach (string kidname in kidNames)
                        {
                            //Construct some employee details
                            Camper newCamper = new Camper()
                            {
                                FirstName = kidname,
                                LastName = lastName,
                                MiddleName = kidname[1].ToString().ToUpper(),
                                DOB = startDOB.AddDays(-random.Next(1480, 5800)),
                                Gender = genders[random.Next(genderCount)],
                                EMail = (kidname.Substring(0, 2) + lastName + random.Next(11, 111).ToString() + "@outlook.com").ToLower(),
                                Phone = (random.Next(2, 10).ToString() + random.Next(213214131, 989898989).ToString()),
                                CompoundID = compoundIDs[random.Next(compoundIDCount)]
                            };
                            campers.Add(newCamper);
                        }
                    }
                    //Now add your list into the DbSet
                    context.Campers.AddRange(campers);
                    context.SaveChanges();
                }
            }
        }
    }

}
