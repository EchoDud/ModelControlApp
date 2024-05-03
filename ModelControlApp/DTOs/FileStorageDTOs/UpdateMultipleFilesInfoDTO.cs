using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileStorageDTOs
{
    public class UpdateMultipleFilesInfoDTO
    {
        public string Owner { get; set; } = null!;
        public BsonDocument UpdatedMetadata { get; set; } = null!;
    }
}
