using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Common
{
    public class RestResponse
    {
        public bool Success { get; private set; }
        public bool IsException { get; private set; }
        public string Data { get; private set; }
        public ErrorResponseModel Error { get; private set; }

        public RestResponse(string data)
        {
            this.Success = true;
            this.IsException = false;
            this.Data = data;
            this.Error = null;
        }

        public RestResponse(ErrorResponseModel error)
        {
            this.Success = false;
            this.IsException = false;
            this.Data = null;
            this.Error = error;
        }

        public RestResponse(Exception exception)
        {
            this.Success = false;
            this.IsException = true;
            this.Data = exception.Message;
            this.Error = null;
        }
    }
}
