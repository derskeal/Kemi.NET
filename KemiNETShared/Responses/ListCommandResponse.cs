using System.Collections.Generic;

namespace KemiNETShared.Responses
{
    public class ListCommandResponse : IResponse
    {
        public ResponseCode Status { get; }
        //public List<string> Data { get; }
        
        public IEnumerable<string> TableHeaders { get; }
        public List<string[]> TableRows { get; }


        public ListCommandResponse(ResponseCode status, IEnumerable<string> tableHeaders, IEnumerable<string[]> tableRows)
        {
            Status = status;
            TableHeaders = tableHeaders;
            TableRows = new List<string[]>(tableRows);
        }
    }
}