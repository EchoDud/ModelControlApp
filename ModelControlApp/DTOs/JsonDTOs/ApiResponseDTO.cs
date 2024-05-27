using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.JsonDTOs
{
    /**
     * @class ApiResponseDTO
     * @brief Data transfer object for API responses.
     */
    public class ApiResponseDTO
    {
        /**
         * @brief Gets or sets the ID of the API response.
         */
        [JsonPropertyName("$id")]
        public string Id { get; set; }

        /**
         * @brief Gets or sets the values returned by the API response.
         */
        [JsonPropertyName("$values")]
        public List<string> Values { get; set; }
    }
}
