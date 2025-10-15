using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Dtos
{
    public class InfoMsg
    {
        public DateTime data { get; set; }
        public string messaggio { get; set; }

        public InfoMsg(DateTime data, string messaggio)
        {        
            this.data = data;
            this.messaggio = messaggio;    
        }
    }
}