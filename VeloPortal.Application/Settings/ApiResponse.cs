using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Settings
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public PaginationMetadata? Pagination { get; set; }

        public ApiResponse(bool success, string message, T? data = default, List<string>? errors = null,
            PaginationMetadata? pagination = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            Pagination = pagination;
        }

        /// <summary>
        /// To Return Success with Data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns>Success Data with Message</returns>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
            => new(true, message, data);
        /// <summary>
        /// To Return Success with data and Pagination
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pagination"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> SuccessResponse(T data, PaginationMetadata pagination, string message = "Success")
    => new(true, message, data, null, pagination);
        public static ApiResponse<T> FailureResponse(List<string> errors, string message = "Error")
            => new(false, message, default, errors);

        public static ApiResponse<T> FailureResponse(string error, string message = "Error")
            => new(false, message, default, new List<string> { error });

    }

    /// <summary>
    /// Holds pagination metadata for API responses
    /// </summary>
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PaginationMetadata(int currentPage, int pageSize, int totalCount)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
