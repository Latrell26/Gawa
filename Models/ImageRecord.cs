using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MauiApp2.Models
{
    [Table("images")]
    public class ImageRecord : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("uploader_name")]
        public string UploaderName { get; set; }

        [Column("title")]
        public string Title { get; set; }
    }
}
