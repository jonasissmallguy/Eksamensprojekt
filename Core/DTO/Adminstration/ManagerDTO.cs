using System.ComponentModel.DataAnnotations;

namespace Core
{

    public class ManagerDTO
    {
        [Required]
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }

    }

}