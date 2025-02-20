using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; } // PK

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty; // HTML 허용

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow; // 자동 생성

        public DateTime? StartDate { get; set; } // 게시 시작일 (선택)
        public DateTime? EndDate { get; set; } // 게시 종료일 (선택)

        [Required]
        [ForeignKey("User")]
        public string ContributorUsername { get; set; } = string.Empty; // 이메일 기반
    }
}
