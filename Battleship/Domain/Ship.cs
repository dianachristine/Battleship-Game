using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Ship
    {
        public int ShipId { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }  = default!;
        
        public ICollection<Coordinate> Coordinates { get; set; }  = default!;
        
        public int GameBoardId { get; set; }
        public GameBoard GameBoard { get; set; } = default!;
    }
}
