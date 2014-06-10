using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ProgramInfo
    {
        public String ProgramId { get; set; }

        public String ProgramName { get; set; }

        public List<FileAttribute> MappingFiles { get; set; }


    }
}
