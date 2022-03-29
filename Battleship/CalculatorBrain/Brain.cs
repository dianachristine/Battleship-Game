using System;

namespace CalculatorBrain
{
    public class Brain
    {
        public Double CurrentValue { get; private set; } = 0;
        
        public void SetValue(double value) => CurrentValue = value;
        
        public double Add(double value) 
        {
            return CurrentValue + value;
        }

        public double Subtract(double value)
        {
            return CurrentValue - value;
        }

        public double Divide(double value)
        {
            if (value.Equals(0.0))
            {
                throw new Exception("Cannot divide with 0!!!");
            }
            return CurrentValue / value;
        }
        
        public double Multiply(double value)
        {
            return CurrentValue * value;
        }
        
        public double Power(double value)
        {
            return Math.Pow(CurrentValue, value);
        }
        
        public double Negate()
        {
            return CurrentValue.Equals(0.0) ? 0: CurrentValue * -1;
        }
        
        public double Sqrt()
        {
            if (CurrentValue < 0)
            {
                throw new Exception($"Cannot get square root from {CurrentValue}!!!");
            }
            return Math.Sqrt(CurrentValue);
        }
                
        public double Square()
        {
            return Math.Pow(CurrentValue, 2);
        }
                        
        public double Abs()
        {
            return Math.Abs(CurrentValue);
        }

        public override string ToString() => CurrentValue.ToString();

    }
}