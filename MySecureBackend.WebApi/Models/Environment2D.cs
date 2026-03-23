using System.ComponentModel.DataAnnotations;

namespace MySecureBackend.WebApi.Models
{
    public class Environment2D
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(1, 10000)]
        public int MaxHeight { get; set; }

        [Range(1, 10000)]
        public int MaxLength { get; set; }

        public float PositionX { get; set; }
        public float PositionY { get; set; }

        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public float RotationZ { get; set; }

        [Required]
        public string SortingLayer { get; set; }
    }
}
