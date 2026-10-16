using System;
using System.Collections.Generic;
using Lab4.Core;
using Xunit;

namespace Lab4.Tests;

public class MathAlgorithmsTests
{
    //Factorial

    [Fact]
    public void Factorial_OfZero_ReturnsOne()
    {
        long result = MathAlgorithms.Factorial(0);

        Assert.Equal(1, result);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 120)]
    [InlineData(10, 3628800)]
    [InlineData(20, 2432902008176640000)] //максимум long
    public void Factorial_ReturnsExpected(int n, long expected)
    {
        Assert.Equal(expected, MathAlgorithms.Factorial(n));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(21)]
    public void Factorial_OutOfRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.Factorial(n));
    }

    //Fibonacci

    [Fact]
    public void Fibonacci_UpTo20_ReturnsCorrectSequence()
    {
        List<long> result = MathAlgorithms.Fibonacci(20);

        Assert.Equal(new long[] { 0, 1, 1, 2, 3, 5, 8, 13 }, result);
    }

    [Fact]
    public void Fibonacci_OfZero_ReturnsOnlyZero()
    {
        List<long> result = MathAlgorithms.Fibonacci(0);

        Assert.Equal(new long[] { 0 }, result);
    }

    [Fact]
    public void Fibonacci_NegativeN_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.Fibonacci(-1));
    }

    //FunctionA (вариант 4)

    // Ожидаемые значения посчитаны отдельно
    [Theory]
    [InlineData(60.0, -2.233412205650)]
    [InlineData(70.0, 0.919634207914)]
    [InlineData(100.0, -0.980302886483)]
    public void FunctionA_ReturnsExpected(double x, double expected)
    {
        Assert.Equal(expected, MathAlgorithms.FunctionA(x), 1e-9);
    }

    [Fact]
    public void FunctionA_AtDomainBoundary_ReturnsValue()
    {
        // Arrange + Act: x = 54 — граница области определения, под корнем ровно 0
        double result = MathAlgorithms.FunctionA(54.0);

        Assert.Equal(-4.504851080007, result, 1e-9);
    }

    [Theory]
    [InlineData(53.9)]
    [InlineData(0.0)]
    [InlineData(-5.0)]
    public void FunctionA_BelowDomain_Throws(double x)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.FunctionA(x));
    }

    [Fact]
    public void FunctionA_SinOfXSquaredIsZero_Throws()
    {
        double x = Math.Sqrt(939 * Math.PI);

        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.FunctionA(x));
    }

    // LnTaylor (вариант 4)
    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(-0.5)]
    [InlineData(0.9)]
    public void LnTaylor_MatchesMathLog(double x)
    {
        Assert.Equal(Math.Log(1 + x), MathAlgorithms.LnTaylor(x), 1e-8);
    }

    [Fact]
    public void LnTaylor_AtUpperBound_ConvergesToLn2()
    {
        double result = MathAlgorithms.LnTaylor(1.0, 1e-3);

        Assert.Equal(Math.Log(2), result, 2e-3);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(-2.0)]
    [InlineData(1.5)]
    public void LnTaylor_OutOfRange_Throws(double x)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.LnTaylor(x));
    }

    [Fact]
    public void LnTaylor_NonPositiveEps_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.LnTaylor(0.5, 0));
    }
}