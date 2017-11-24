using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    public class ErrorMessage
    {
        public string Module { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public string SystemMessage { get; set; }

        public ErrorMessage(string module, int code, string message, string systemMessage = null)
        {
            Module = module;
            Code = code;
            Message = message;
            SystemMessage = systemMessage;
        }

        public override string ToString()
        {
            string ReturnMessage = Module + Code.ToString("000") + ": " + Message;

            if (!string.IsNullOrWhiteSpace(SystemMessage))
                ReturnMessage += " (" + SystemMessage + ")";

            return ReturnMessage;
        }
    }

}
