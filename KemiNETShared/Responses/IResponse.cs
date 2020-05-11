using System;
using System.Collections.Generic;

namespace KemiNETShared.Responses
{
    public interface IResponse
    {
        ResponseCode Status { get; }

        //List<string> Data { get; }
    }
    
    public enum ResponseCode
    {
        SUCCESS, ERROR
    }
}