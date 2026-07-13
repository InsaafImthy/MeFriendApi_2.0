using MeFriendApi.Domain.Exceptions;
using static MeFriendApi.Domain.Dto.Helpers.CommonDto;

namespace MeFriendApi.Services.Tools
{
    public class ExceptionHandler
    {
        public ResponseModel GetStatusException(Exception exception)
        {
            try
            {
                if (exception is AccessException) return new ResponseModel { StatusCode = 401, Message = exception.Message };
                else if (exception is InternalException) return new ResponseModel { StatusCode = 500, Message = exception.Message };
                else if (exception is ForbiddenException) return new ResponseModel { StatusCode = 403, Message = exception.Message };
                else if (exception is BadRequestException) return new ResponseModel { StatusCode = 400, Message = exception.Message };
                else if (exception is ConflictException) return new ResponseModel { StatusCode = 409, Message = exception.Message };
                else if (exception is NotFoundException) return new ResponseModel { StatusCode = 404, Message = exception.Message };
                else return new ResponseModel { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new ResponseModel { StatusCode = 500 };
            }
        }
    }
}
