using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CalculatorBrain;
using MenuSystem;

namespace Calculator
{
    class CalculatorProgram
    {
        private static readonly Brain Brain = new Brain();

        static void Main(string[] args)
        {
            Console.Clear();

            var mainMenu = new Menu(ReturnCurrentDisplayValue,"Main Menu", EMenuLevel.Root);
            
            mainMenu.SetMenuForegroundColor(ConsoleColor.Yellow);
            var banner = File.ReadAllText(@"../../../banner.txt");
            mainMenu.ShowMenuBanner(banner);

            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("B", "Binary operations", SubmenuBinary),
                new MenuItem("U", "Unary operations", SubmenuUnary),
            });
            
            mainMenu.Run();
        }
        
        public static string ReturnCurrentDisplayValue()
        {
            return Brain.CurrentValue.ToString();
        }

        private static string SubmenuBinary()
        {
            var menu = new Menu(ReturnCurrentDisplayValue,"Binary", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("+", "+", Add),
                new MenuItem("-", "-", Subtract),
                new MenuItem("/", "/", Divide),
                new MenuItem("*", "*", Multiply),
                new MenuItem("**", "**", Power),

            });
            var res = menu.Run();
            return res;
        }
        
        private static string SubmenuUnary()
        {
            var menu = new Menu(ReturnCurrentDisplayValue,"Unary", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("N", "Negate", Negate),
                new MenuItem("Q", "Sqrt", Sqrt),
                new MenuItem("S", "Square", Square),
                new MenuItem("A", "Abs", Abs),
            });
            var res = menu.Run();
            return res;
        }

        private static string Add()
        {
            return DoMathWithTwo("Add number to current value","+", Brain.Add);
        }
        
        private static string Subtract()
        {
            return DoMathWithTwo("Subtract number from current value","-", Brain.Subtract);
        }
        
        private static string Divide()
        {
            return DoMathWithTwo("Divide current value with number","/", Brain.Divide);
        }
        
        private static string Multiply()
        {
            return DoMathWithTwo("Multiply number with current value","*", Brain.Multiply);
        }
        
        private static string Power()
        {
            return DoMathWithTwo("Power current value with number","**", Brain.Power);
        }
        
        private static string Negate()
        {
            return DoMathWithOne("Negate", Brain.Negate);
        }
        
        private static string Sqrt()
        {
            return DoMathWithOne("Square Root", Brain.Sqrt);
        }
                
        private static string Square()
        {
            return DoMathWithOne("Square", Brain.Square);
        }
                        
        private static string Abs()
        {
            return DoMathWithOne("Absolute value", Brain.Abs);
        }
        

        private static string DoMathWithTwo(string info, string operation, Func<double, double> func)
        {
            ShowCurrentValue();
            Console.WriteLine(info);
            
            var convertedUserInput = GetUserInput();

            try
            {
                var result = func(convertedUserInput);
                ShowExpressionForBinary(convertedUserInput, result, operation);
                Brain.SetValue(result);
            }
            catch (Exception e)
            {
                HandleMathException(e);
            }
            return "";
        }
        
        private static string DoMathWithOne(string operation, Func<double> func)
        {
            ShowCurrentValue();

            try
            {
                var result = func();
                ShowExpressionForUnary(result, operation);
                Brain.SetValue(result);
            }
            catch (Exception e)
            {
                HandleMathException(e);
            }
            return "";
        }
        
        private static double GetUserInput()
        {
            Console.Write("Number: ");
            var input = Console.ReadLine()?.Trim();
            double.TryParse(input, out var converted);
            return converted;
        }

        private static void ShowCurrentValue()
        {
            Console.WriteLine("");
            Console.WriteLine("Current value: " + Brain.CurrentValue);
        }
        
        private static void ShowExpressionForBinary(double convertedUserInput, double result, string sign)
        {
            var expression = $"|   {Brain.CurrentValue} {sign} {convertedUserInput} = {result}   |";
            ShowExpression(expression);
        }
            
        private static void ShowExpressionForUnary(double result, string operation)
        {
            var expression = $"|   {operation} of {Brain.CurrentValue} is {result}   |";
            ShowExpression(expression);
        }

        private static void ShowExpression(string expression)
        {
            var expressionSeparator = new String('-', expression.Length);
            
            Console.WriteLine("");
            Console.WriteLine(expressionSeparator);
            Console.WriteLine(expression);
            Console.WriteLine(expressionSeparator);
            
            Sleep(1000);
        }

        private static void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout); // for nicer UX
        }

        private static void HandleMathException(Exception e)
        {
            Console.WriteLine(e.Message);
            Sleep(1000);
        }
    }
}
