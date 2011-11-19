using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMakerConnect
{
    public class FileMakerFieldAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Updateable { get; set; }

        public FileMakerFieldAttribute(string Name) : this(Name, true) { }

        public FileMakerFieldAttribute(string Name, bool Updateable)
        {
            this.Name = Name;
            this.Updateable = Updateable;
        }
    }
}
