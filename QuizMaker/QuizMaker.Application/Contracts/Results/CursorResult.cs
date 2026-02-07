using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Application.Common.Results;

/// <summary>
/// Represents a paginated result set using cursor-based pagination.
/// </summary>
/// <typeparam name="T">The type of items contained in the result set.</typeparam>
public class CursorResult<T> {

    public IEnumerable<T> Items { get; set; } = null!;
    public bool HasNextPage { get; set; }

    public string? NextCursor { get; set; }
}
