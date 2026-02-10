using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.UnitTests.Domain;
public static class MemberDataForTests {

    public static IEnumerable<object?[]> InvalidStrings =>
        new[]
        {
            new object?[] { null },
            new object?[] { "" },
        };

    public static IEnumerable<object?[]> InvalidTitleAndMaxQuestions =>
    new[]
    {
        new object?[] { "Test", null },
        new object?[] { "Test", "" },
        new object?[] { null, "Test" },
        new object?[] { "", "Test" },
    };
}
