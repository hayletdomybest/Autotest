using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdate
{
    public enum JobType
    {
        UPDATE,
        REMOVE
    }

    public class Element
    {
        //Private innounce
        public string _Date;

        public string _Uri;

        public string _FileName;
    }

    public class UpdateItem
    {
        public string _Name;
        public string _Path;
    }
}
