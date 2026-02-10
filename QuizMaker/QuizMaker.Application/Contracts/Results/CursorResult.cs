using System.Collections.Generic;

namespace QuizMaker.Application.Common.Results;

/// <summary>
/// Represents a generic result set for cursor-based pagination.
/// This approach avoids the performance pitfalls of offset-based pagination (skip/take) on large datasets.
/// </summary>
/// <typeparam name="T">The type of items contained in the result set (e.g., DTOs).</typeparam>
public class CursorResult<T> {
    /// <summary>
    /// The collection of items returned for the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = null!;

    /// <summary>
    /// Indicates whether there are more items available beyond this page.
    /// <br/>If <b>true</b>, the client should use the <see cref="NextCursor"/> to fetch the next set of results.
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// The opaque cursor string pointing to the start of the next page.
    /// <br/>Pass this value as the 'cursor' parameter in the next API request.
    /// <br/>If <see cref="HasNextPage"/> is <b>false</b>, this value is usually null.
    /// </summary>
    public string? NextCursor { get; set; }
}