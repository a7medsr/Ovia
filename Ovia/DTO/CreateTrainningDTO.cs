using System.ComponentModel.DataAnnotations;

namespace Ovia.DTO
{
    public class CreateTrainningDTO
    {
        public string TrainningName { get; set; }
        public DateTime Date { get; set; }
        public int CreatedBy { get; set; }

    }
}
