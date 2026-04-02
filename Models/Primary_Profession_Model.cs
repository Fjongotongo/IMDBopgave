using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Models
{
    public class Primary_Profession_Model
    {
       public string PrimaryProfession { get; set; }

       public Primary_Profession_Model(string primaryProfession)
        {
            PrimaryProfession = primaryProfession;
        }
    }
}
