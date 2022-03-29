using System.Collections.Generic;

namespace BattleShipBrain
{
    public class GameBoard
    {
        public BoardSquareState[,] Board { get; set; } = null!;
        public List<Ship> Ships { get; set; } = default!;

        public static SaveGameDTO.GameBoardDTO[] ConvertGameBoardsToGameBoardsDtos(GameBoard[] gameBoards)
        {
            var gameBoardDtos = new SaveGameDTO.GameBoardDTO[2];
            
            for (var i = 0; i < 2; i++)
            {
                gameBoardDtos[i] = new SaveGameDTO.GameBoardDTO
                {
                    Board = new List<List<BoardSquareState>>(), 
                    Ships = gameBoards[i].Ships
                };

                for (var x = 0; x < gameBoards[i].Board.GetLength(0); x++)
                {
                    List<BoardSquareState> innerList = new List<BoardSquareState>();
                    
                    for (var y = 0; y < gameBoards[i].Board.GetLength(1); y++)
                    {
                        innerList.Add(gameBoards[i].Board[x, y]);
                    }
                    
                    gameBoardDtos[i].Board.Add(innerList);
                }
            }
            return gameBoardDtos;
        }
    }
}
