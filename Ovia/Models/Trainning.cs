using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public class Trainning : Entity
    {
        [Key]
        public int Id { get; set; }
        public string TrainningName { get; set; }
        public string TrainningCode { get; set; }
        public DateTime Date { get; set; }

        //public string Url { get; set; }



    }
}
