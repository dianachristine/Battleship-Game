using System.Collections.Generic;

namespace BattleShipBrain
{
    public class SaveGameDTO
    {
        public int CurrentPlayerNo { get; set; } = 0;
        public GameBoardDTO[] GameBoards  { get; set; } = new GameBoardDTO[2];
        public GameConfig GameConfig { get; set; } = new GameConfig();
        
        public SaveGameDTO()
        {
        }  
        public SaveGameDTO(int currentPlayerNo, GameConfig gameConfig, GameBoard[] gameBoards)
        {
            CurrentPlayerNo = currentPlayerNo;
            GameConfig = gameConfig;
            GameBoards = GameBoard.ConvertGameBoardsToGameBoardsDtos(gameBoards);
        }

        public static GameBoard[] ConvertSaveGameDtosToGameBoards(GameBoardDTO[] dtos)
        {
            GameBoard[] gameBoards = new GameBoard[2];

            for (var i = 0; i < 2; i++)
            {
                gameBoards[i] = new GameBoard();
                gameBoards[i].Board = new BoardSquareState[dtos[0].Board.Count, dtos[0].Board[0].Count];
                gameBoards[i].Ships = new List<Ship>();

                for (var x = 0; x < dtos[0].Board.Count; x++)
                {
                    for (var y = 0; y < dtos[0].Board[0].Count; y++)
                    {
                        gameBoards[i].Board[x, y] = dtos[i].Board[x][y];
                    }
                }

                foreach (var ship in dtos[i].Ships)
                {
                    gameBoards[i].Ships.Add(ship);
                }
            }

            return gameBoards;
        }
        
        public class GameBoardDTO
        {
            public List<List<BoardSquareState>> Board { get; set; } = null!;
            public List<Ship> Ships { get; set; } = null!;
        }
        
    }
}
