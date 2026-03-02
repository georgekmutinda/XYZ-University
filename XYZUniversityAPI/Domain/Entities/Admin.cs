using System.Collections.Generic;
namespace XYZUniversityAPI.Domain.Entities

{ public class Admin

    {
        public int AdminId { get; set; }
        public string AdminName { get; set; } = null!;
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
   
}
