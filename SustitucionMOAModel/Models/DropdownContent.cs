using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models
{
    public class DropdownContent
    {
        public List<DropdownOption> options { get; set; }

        public DropdownContent() {
            this.options = new List<DropdownOption>() { new DropdownOption() { value = "", label = "Todos" } };
        }

        public DropdownContent(List<DropdownOption> options)
        {
            this.options = new List<DropdownOption>() { new DropdownOption() { value = "", label = "Todos" } };
            this.options = this.options.Concat(options).ToList();
        }
    }
}
