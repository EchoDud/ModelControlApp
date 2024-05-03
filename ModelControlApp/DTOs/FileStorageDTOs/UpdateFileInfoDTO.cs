using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileStorageDTOs
{
    public class UpdateFileInfoDTO : DataFileWithoutVersionDTO
    {
        public BsonDocument UpdatedMetadata { get; set; } = null!;
    }
}
