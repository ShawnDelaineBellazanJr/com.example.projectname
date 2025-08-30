using System.ComponentModel;
using ModelContextProtocol.Server;

namespace BasicCalculator.Tools;

/// <summary>
/// Basic Calculator MCP Server implementation for ThoughtTransfer samples.
/// 
/// This class demonstrates how to create a simple MCP server with calculator tools
/// that can perform basic arithmetic operations and demonstrate MCP integration patterns.
/// </summary>
[McpServerToolType]
public static class BasicCalculatorServer
{
    [McpServerTool, Description("Calculates the sum of two numbers")]
    public static double Add(double numberA, double numberB)
    {
        Console.WriteLine($"🔢 Computing: {numberA} + {numberB}");
        var result = numberA + numberB;
        Console.WriteLine($"✅ Result: {result}");
        return result;
    }

    [McpServerTool, Description("Calculates the difference of two numbers")]
    public static double Subtract(double numberA, double numberB)
    {
        Console.WriteLine($"🔢 Computing: {numberA} - {numberB}");
        var result = numberA - numberB;
        Console.WriteLine($"✅ Result: {result}");
        return result;
    }

    [McpServerTool, Description("Calculates the product of two numbers")]
    public static double Multiply(double numberA, double numberB)
    {
        Console.WriteLine($"🔢 Computing: {numberA} × {numberB}");
        var result = numberA * numberB;
        Console.WriteLine($"✅ Result: {result}");
        return result;
    }

    [McpServerTool, Description("Calculates the quotient of two numbers")]
    public static double Divide(double numberA, double numberB)
    {
        Console.WriteLine($"🔢 Computing: {numberA} ÷ {numberB}");
        if (numberB == 0)
        {
            Console.WriteLine("❌ Error: Cannot divide by zero");
            throw new ArgumentException("Cannot divide by zero");
        }
        var result = numberA / numberB;
        Console.WriteLine($"✅ Result: {result}");
        return result;
    }

    [McpServerTool, Description("Calculates the power of a base raised to an exponent")]
    public static double Power(double baseNumber, double exponent)
    {
        Console.WriteLine($"🔢 Computing: {baseNumber}^{exponent}");
        var result = Math.Pow(baseNumber, exponent);
        Console.WriteLine($"✅ Result: {result}");
        return result;
    }

    [McpServerTool, Description("Calculates the square root of a number")]
    public static double SquareRoot(double number)
    {
        Console.WriteLine($"🔢 Computing: √{number}");
        if (number < 0)
        {
            Console.WriteLine("❌ Error: Cannot calculate square root of negative number");
            throw new ArgumentException("Cannot calculate square root of negative number");
        }
        var result = Math.Sqrt(number);
        Console.WriteLine($"✅ Result: {result}");
        return result;
    }
    
    [McpServerTool, Description("Validates if a number is prime")]
    public static bool IsPrime(long number)
    {
        Console.WriteLine($"🔍 Checking if {number} is prime...");
        
        if (number <= 1) 
        {
            Console.WriteLine("❌ Numbers ≤ 1 are not prime");
            return false;
        }
        if (number <= 3) 
        {
            Console.WriteLine("✅ Numbers 2 and 3 are prime");
            return true;
        }
        if (number % 2 == 0 || number % 3 == 0) 
        {
            Console.WriteLine("❌ Number is divisible by 2 or 3, not prime");
            return false;
        }

        // Check divisibility using the 6k±1 optimization
        for (long i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
            {
                Console.WriteLine($"❌ Number is divisible by {i} or {i + 2}, not prime");
                return false;
            }
        }

        Console.WriteLine("✅ Number is prime!");
        return true;
    }

    [McpServerTool, Description("Generates Fibonacci sequence up to n terms")]
    public static long[] Fibonacci(int count)
    {
        Console.WriteLine($"🔢 Generating Fibonacci sequence with {count} terms");
        
        if (count <= 0)
        {
            Console.WriteLine("❌ Count must be positive");
            throw new ArgumentException("Count must be positive");
        }

        var sequence = new long[count];
        if (count >= 1) sequence[0] = 0;
        if (count >= 2) sequence[1] = 1;

        for (int i = 2; i < count; i++)
        {
            sequence[i] = sequence[i - 1] + sequence[i - 2];
        }

        Console.WriteLine($"✅ Generated sequence: [{string.Join(", ", sequence)}]");
        return sequence;
    }
}
