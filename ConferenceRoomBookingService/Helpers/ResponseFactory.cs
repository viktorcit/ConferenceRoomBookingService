using ConferenceRoomBookingService.Data.DTO;
using ConferenceRoomBookingService.Enums;

namespace ConferenceRoomBookingService.Helpers
{
    public class ResponseFactory
    {
        public static BaseResponseDto Ok(string message)
        {
            return new BaseResponseDto
            {
                IsSuccess = true,
                ResponseType = ResponseType.None,
                ResponseMessage = message
            };
        }

        public static BaseResponseDto<T> Ok<T>(T data)
        {
            return new BaseResponseDto<T>
            {
                IsSuccess = true,
                ResponseType = ResponseType.None,
                Data = data
            };
        }

        public static BaseResponseDto<T> Fail<T>(ResponseType responseType, string message)
        {
            return new BaseResponseDto<T>
            {
                IsSuccess = false,
                ResponseType = responseType,
                ResponseMessage = message,
                Data = default
            };
        }

        public static BaseResponseDto Fail(ResponseType responseType, string message)
        {
            return new BaseResponseDto
            {
                IsSuccess = false,
                ResponseType = responseType,
                ResponseMessage = message
            };
        }
    }
}
