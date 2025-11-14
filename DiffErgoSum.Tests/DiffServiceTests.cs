namespace DiffErgoSum.Tests;

using System.Text;

using DiffErgoSum.Core;

public class DiffAlgorithmTests
{
    [Fact]
    public void Should_Return_Equals_When_Both_Inputs_Are_Identical()
    {
        var left = Encoding.UTF8.GetBytes("AAAA");
        var right = Encoding.UTF8.GetBytes("AAAA");

        var result = DiffAlgorithm.Compare(left, right);

        Assert.Equal(DiffType.Equals, result.Type);
        Assert.Empty(result.Diffs);
    }

    [Fact]
    public void Should_Return_SizeDoNotMatch_When_Lengths_Differ()
    {
        var left = Encoding.UTF8.GetBytes("AAAA");
        var right = Encoding.UTF8.GetBytes("AAA");

        var result = DiffAlgorithm.Compare(left, right);

        Assert.Equal(DiffType.SizeDoNotMatch, result.Type);
    }

    [Fact]
    public void Should_Return_ContentDoNotMatch_With_Offsets()
    {
        var left = Encoding.UTF8.GetBytes("AAAA");
        var right = Encoding.UTF8.GetBytes("ABAA");

        var result = DiffAlgorithm.Compare(left, right);

        Assert.Equal(DiffType.ContentDoNotMatch, result.Type);
        Assert.Single(result.Diffs);
        Assert.Equal(1, result.Diffs[0].Offset);
        Assert.Equal(1, result.Diffs[0].Length);
    }
}
