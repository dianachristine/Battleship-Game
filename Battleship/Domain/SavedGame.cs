using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SavedGame
    {
        public int SavedGameId { get; set; }
        
        [MaxLength(150)]
        public string SavedGameName { get; set; }  = default!;
        
        public int CurrentPlayerNo { get; set; }
        
        public ICollection<GameBoard> GameBoards { get; set; }  = default!;
        
        public int GameConfigId { get; set; }
        public GameConfig GameConfig { get; set; } = default!;
    }
}
