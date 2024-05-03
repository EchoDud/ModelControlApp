using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileStorageDTOs
{
    public class UpdateFileInfoByVersionDTO : DataFileWithVersionDTO
    {
        public BsonDocument UpdatedMetadata { get; set; } = null!;
    }
}
