using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Models
{
    public class Name_Model
    {
        public int NConst { get; set; }
        public string PrimaryName { get; set; }
        public int? BirthYear { get; set; }
        public int? DeathYear { get; set; }

        public Name_Model(string[] namesInfo)
        {
            NConst = int.Parse(namesInfo[0].Substring(2));
            PrimaryName = namesInfo[1];
            BirthYear = ParseNullableInt(namesInfo[2]);
            DeathYear = ParseNullableInt(namesInfo[3]);
        }

        private int? ParseNullableInt(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return null;
        }

        public override string ToString()
        {
            return NConst + " - " + PrimaryName;
        }
    }
}
