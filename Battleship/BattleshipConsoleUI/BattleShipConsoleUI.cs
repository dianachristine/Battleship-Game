using System;
using System.Collections.Generic;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public class BSConsoleUI : GameConfigsUI
    {
        // game
        public void WelcomeAndShowShipInfo()
        {
            Console.WriteLine();
            Console.WriteLine();
            SetConsoleColor(ConsoleColor.DarkGreen);
            Console.WriteLine("Welcome to the Battleship game! ");
            
            SetConsoleColor(ConsoleColor.DarkCyan);
            Console.WriteLine("To play the game both players have to place the ships on board.");
            Console.WriteLine();
            
            SetConsoleColorBackToMenuColor();
        }

        public void ShowWhichPlayerHasToPLaceTheShips(string player)
        {
            SetConsoleColor(ConsoleColor.DarkCyan);
            Console.WriteLine($"PLayer {player}, please place your ships");
            Console.WriteLine();
            
            SetConsoleColorBackToMenuColor();
        }
        
        public void ShowUserShipPlacementOptions(string player)
        {
            Console.WriteLine($"Player {player} please choose how you want to place your ships.");
            Console.WriteLine("M - place ships manually");
            Console.WriteLine("R - place ships randomly");
            Console.Write(">");
        }
        
        public void ShowShipAndGameConfigsInfo(GameConfig gameConfig, ShipConfig shipConfig)
        {
            SetConsoleColor(ConsoleColor.DarkCyan);
            Console.WriteLine();
            Console.WriteLine($"Touch Rule is - {gameConfig.EShipTouchRule.ToString()}");
            Console.WriteLine($"Ship to place - name: {shipConfig.Name}, Size1: {shipConfig.ShipSizeX}, Size2: {shipConfig.ShipSizeY}, Number of these ships: {shipConfig.Quantity}");
            
            SetConsoleColorBackToMenuColor();
        }
        
        public void ShowShipCoordinateInfo(string coordinateType)
        {
            Console.WriteLine($"Please give a {coordinateType} coordinate for your ship.");
            Console.Write("> ");
        }
        
        public void InformThatShipsAreSuccessfullyPlaced()
        {
            SetConsoleColor(ConsoleColor.DarkGreen);
            Console.WriteLine("Ships for both players are successfully placed! The game starts now!");
            
            SetConsoleColorBackToMenuColor();
        }
        
        public void ShowBoardsAndGameInfoToUser(BoardSquareState[,] board, BoardSquareState[,] opponentBoard, string playerAorB)
        {
            SetConsoleColor(ConsoleColor.DarkCyan);
            Console.WriteLine();
            DrawPLayerAndOpponentBoards(board, opponentBoard);
            ShowGameInfo(playerAorB, board.GetLength(0), board.GetLength(1));
            
            SetConsoleColorBackToMenuColor();
            GiveMoveInfoToUser();
        }
        
        public void ShowGameInfo(string playerAorB, int x, int y)
        {
            Console.WriteLine($"Player {playerAorB} make a move");
            Console.WriteLine($"Upper left corner coordinate is (A1) and board size is {x}x{y}.");
        }

        public void GiveMoveInfoToUser()
        {
            Console.WriteLine("(Write S to save, E to exit, U to undo previous move)");
            Console.WriteLine("Give X and Y  (e.g.: a1, c4, etc): ");
            Console.Write(">");
        }

        public void ShowMoveResultAndContinueMessage(string result, string nextPlayer)
        {
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            SetConsoleColor(ConsoleColor.Yellow);
            Console.WriteLine($"{result}");
            
            SetConsoleColorBackToMenuColor();
            Console.WriteLine($"Player {nextPlayer} please press ENTER to continue");
            Console.Write(">");
        }
        
        public void ShowWinningMessage(string winner)
        {
            Console.WriteLine("Game over!");
            SetConsoleColor(ConsoleColor.Yellow);
            Console.WriteLine($"PLAYER {winner} WINS! Congratulations!");
            
            SetConsoleColorBackToMenuColor();
            Console.WriteLine();
        }
        
        // boards 
        public void DrawPLayerBoard(BoardSquareState[,] board)
        {
            SetConsoleColor(ConsoleColor.DarkCyan);
            var x = board.GetLength(0); // width
            var y = board.GetLength(1); // height
           
            WriteLettersOnTopOfTheBoard(x);
            Console.WriteLine();
            
            WriteRowSeparator(x);
            Console.WriteLine();

            for (var rowIndex = 0; rowIndex < y; rowIndex++)
            {
                Console.Write(rowIndex < 9 ? $"  {rowIndex + 1} ||" : $" {rowIndex + 1} ||");

                for (var colIndex = 0; colIndex < x; colIndex++)
                {
                    Console.Write($" {board[colIndex, rowIndex]} |");
                }
                Console.WriteLine();
                
                WriteRowSeparator(x);
                Console.WriteLine();
            }
            SetConsoleColorBackToMenuColor();
        }        
        public void DrawPLayerAndOpponentBoards(BoardSquareState[,] board, BoardSquareState[,] opponentBoard)
        {
            var x = board.GetLength(0); // width
            var y = board.GetLength(1); // height

            const string separator = "       ";

            Console.WriteLine($"YOUR BOARD:{new string(' ', x * 4)}  YOUR OPPONENT BOARD:");
            
            WriteLettersOnTopOfTheBoard(x);
            Console.Write(separator);
            WriteLettersOnTopOfTheBoard(x);
            Console.WriteLine();
            
            WriteRowSeparator(x);
            Console.Write(separator);
            WriteRowSeparator(x);
            Console.WriteLine();

            for (var rowIndex = 0; rowIndex < y; rowIndex++)
            {
                Console.Write(rowIndex < 9 ? $"  {rowIndex + 1} ||" : $" {rowIndex + 1} ||");
                for (var colIndex = 0; colIndex < x; colIndex++)
                {
                    if (board[colIndex, rowIndex].IsShip && board[colIndex, rowIndex].IsBomb)
                    {
                        SetConsoleColor(ConsoleColor.DarkMagenta);
                        Console.Write($" {board[colIndex, rowIndex]} ");
                        
                        SetConsoleColorBackToGameColor();
                        Console.Write("|");
                    }
                    else
                    {
                        Console.Write($" {board[colIndex, rowIndex]} |");
                    }
                }
                Console.Write(separator);
                
                Console.Write(rowIndex < 9 ? $"  {rowIndex + 1} ||" : $" {rowIndex + 1} ||");
                for (var colIndex = 0; colIndex < x; colIndex++)
                {
                    if (opponentBoard[colIndex, rowIndex].IsShip && opponentBoard[colIndex, rowIndex].IsBomb)
                    {
                        SetConsoleColor(ConsoleColor.DarkRed);
                        
                        Console.Write($" {opponentBoard[colIndex, rowIndex]} ");
                        SetConsoleColorBackToGameColor();
                        Console.Write("|");
                    }
                    else if (opponentBoard[colIndex, rowIndex].IsShip && !opponentBoard[colIndex, rowIndex].IsBomb)
                    {
                        SetConsoleColor(ConsoleColor.Gray);
                        Console.Write($" {opponentBoard[colIndex, rowIndex]} ");
                        
                        SetConsoleColorBackToGameColor();
                        Console.Write("|");
                    }
                    else
                    {
                        Console.Write($" {opponentBoard[colIndex, rowIndex]} |");
                    }
                }
                
                Console.WriteLine();
                
                WriteRowSeparator(x);
                Console.Write(separator);
                WriteRowSeparator(x);
                Console.WriteLine();
            }
        }
        
        private static void WriteLettersOnTopOfTheBoard(int x)
        {
            char[] letters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 
                'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};  

            Console.Write("    ||");
            for (var colIndex = 0; colIndex < x; colIndex++)
            {
                Console.Write($" {letters[colIndex]} |");
            }
        }

        private static void WriteRowSeparator(int x)
        {
            for (var colIndex = 0; colIndex <= x; colIndex++)
            {
                Console.Write(colIndex == 0 ? $"----++" : $"---+");
            }
        }

        // saving and loading
        public void ShowFilesToUser(List<string> files)
        {
            Console.WriteLine();
            for (var i = 0; i < files.Count; i++)
            {
                var filesArray = files[i].Split("/");
                Console.WriteLine($"{i} - {filesArray[^1]}");  // element at last index
            }
        }        
        
        public void ShowConfigsFromDbToUser(List<Domain.GameConfig> configs)
        {
            Console.WriteLine();
            foreach (var config in configs)
            {
                Console.WriteLine($"{configs.IndexOf(config)} - {config.GameConfigName}");
            }
        }
        
        public void ShowGamesFromDbToUser(List<Domain.SavedGame> games)
        {
            Console.WriteLine();
            foreach (var game in games)
            {
                Console.WriteLine($"{games.IndexOf(game)} - {game.SavedGameName}");
            }
        }
        
        public void ShowCustomOrDefaultNameInfo(string info)
        {
            Console.WriteLine();
            Console.WriteLine($"Press Enter for default {info} name or type custom name");
            Console.Write(">");
        }
        
        // colours
        public static void SetConsoleColorBackToMenuColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
        }    
        public static void SetConsoleColorBackToGameColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
        }    
        public static void SetConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        // basic stuff
        public void ShowInfo(string msg)
        {
            Console.WriteLine(msg);
        }
        
        public void ShowInfoWithoutNewline(string msg)
        {
            Console.Write(msg);
        }
        
        public void WriteNewline()
        {
            Console.WriteLine();
        }
    }
}
