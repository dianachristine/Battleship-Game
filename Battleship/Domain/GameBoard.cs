using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameBoard
    {
        public int GameBoardId { get; set; }
        
        [MaxLength(21000)]
        public string Board { get; set; }  = default!;
        
        public ICollection<Ship> Ships { get; set; }  = default!;
        
        public int SavedGameId { get; set; }
        public SavedGame SavedGame { get; set; } = default!;
    }
}
