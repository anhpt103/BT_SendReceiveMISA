using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BT_ReceiveDataMISA.Entities
{
    public class CauHinhDongBo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChuKyThucHien { get; set; }

        [Required]
        [MaxLength(200)]
        [JsonIgnore]
        [Column(TypeName = "varchar(200)")]
        public string Token { get; set; }
    }
}
