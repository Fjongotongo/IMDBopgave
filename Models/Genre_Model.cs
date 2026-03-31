using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Models
{
    public class Genre_Model
    {
        public string Genre { get; set; }

        public Genre_Model(string genre)
        {
            Genre = genre;
        }
    }
}
