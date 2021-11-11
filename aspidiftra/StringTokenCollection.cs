using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspidiftra
{
	/// <summary>
	///   Collection of <see cref="StringToken" />s. Has lots of handy built-in functionality.
	/// </summary>
	internal class StringTokenCollection : IEnumerable<StringToken>
	{
		private readonly IEnumerable<StringToken> _tokens;

		/// <summary>
		///   Constructor.
		/// </summary>
		internal StringTokenCollection()
		{
			_tokens = new List<StringToken>();
		}

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="stringToTokenize">Creates a token collection from the given string.</param>
		internal StringTokenCollection(string stringToTokenize)
		{
			var tokens = new List<StringToken>();
			// Platform-neutralize the string with regard to line breaks.
			// Also remove leading or trailing whitespace.
			stringToTokenize = stringToTokenize.Replace("\r\n", "\n").Trim();
			var chars = stringToTokenize.ToCharArray();
			var currentString = new StringBuilder();
			StringToken.TokenType? currentTokenType = null;

			void AddCurrentToken()
			{
				if (currentString.Length > 0 && currentTokenType != null)
				{
					tokens.Add(new StringToken(currentString.ToString(), (StringToken.TokenType) currentTokenType));
					currentString = new StringBuilder();
				}
			}

			foreach (var chr in chars)
			{
				var thisCharTokenType = StringToken.GetTokenTypeForChar(chr);
				if (thisCharTokenType != currentTokenType || currentTokenType == StringToken.TokenType.LineBreak)
					AddCurrentToken();
				currentString.Append(chr);
				currentTokenType = thisCharTokenType;
			}

			AddCurrentToken();
			_tokens = new StringTokenCollection(tokens).Normalize();
		}

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="tokens">
		///   Constructs a token collection from the given set of tokens.
		///   This collection is NOT normalized.
		/// </param>
		internal StringTokenCollection(IEnumerable<StringToken> tokens)
		{
			_tokens = tokens;
		}

		/// <summary>
		///   Could the string that would be created from this token collection
		///   be split into multiple pieces?
		/// </summary>
		internal bool IsSplittable => _tokens.Count(token => token.Type == StringToken.TokenType.Text) > 1;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _tokens.GetEnumerator();
		}

		public IEnumerator<StringToken> GetEnumerator()
		{
			return _tokens.GetEnumerator();
		}

		public static StringTokenCollection operator +(StringTokenCollection collection1, StringTokenCollection collection2)
		{
			var list = collection1._tokens.ToList();
			list.AddRange(collection2._tokens);
			return new StringTokenCollection(list);
		}

		/// <summary>
		///   Returns multiple string token collections, representing each line that
		///   this string token collection contains. The resulting collections are
		///   guaranteed not to contain any line break tokens.
		/// </summary>
		/// <returns>Token collections representing lines of text.</returns>
		internal IEnumerable<StringTokenCollection> GetLineCollections()
		{
			var tokenList = _tokens.ToList();
			var lineBreakIndices = tokenList.Select((token, index) =>
			{
				if (token.Type == StringToken.TokenType.LineBreak)
					return index;
				return (int?) null;
			}).Where(index => index != null).Cast<int>();
			var collections = new List<StringTokenCollection>();
			var startIndex = 0;
			foreach (var lineBreakIndex in lineBreakIndices)
			{
				collections.Add(new StringTokenCollection(tokenList.GetRange(startIndex, lineBreakIndex - startIndex)));
				startIndex = lineBreakIndex + 1;
			}

			collections.Add(new StringTokenCollection(tokenList.GetRange(startIndex, tokenList.Count - startIndex)));

			return collections;
		}

		/// <summary>
		///   Ensures that no consecutive pair of tokens are whitespace and line break by removing
		///   the whitespace. Also ensures that the collection does not start or end with
		///   whitespace.
		/// </summary>
		/// <returns>Normalized collection.</returns>
		internal StringTokenCollection Normalize()
		{
			var tokenList = _tokens.ToList();
			var indicesToRemove = tokenList.Select((token, index) =>
			{
				if (token.Type == StringToken.TokenType.Whitespace)
				{
					if (index == 0 || index == tokenList.Count - 1)
						return index;
					if (tokenList[index - 1].Type == StringToken.TokenType.LineBreak)
						return index;
					if (tokenList[index + 1].Type == StringToken.TokenType.LineBreak)
						return index;
				}

				return (int?) null;
			}).Where(index => index != null).Cast<int>().Reverse();
			foreach (var index in indicesToRemove)
				tokenList.RemoveAt(index);
			return new StringTokenCollection(tokenList);
		}

		/// <summary>
		///   Gets the first group of tokens that represent "content". Basically returns the tokens
		///   from the start of the list that contain one non-whitespace token.
		/// </summary>
		/// <returns>
		///   A tuple containing two new token collections: the "content" tokens, and the
		///   remainder of the tokens.
		/// </returns>
		internal (StringTokenCollection, StringTokenCollection) GetNextContent()
		{
			var thisAsList = this.ToList();
			var firstNonWhitespaceIndex = thisAsList.FindIndex(token => token.Type != StringToken.TokenType.Whitespace);
			var contentListSize = firstNonWhitespaceIndex + 1;
			var contentList = thisAsList.GetRange(0, contentListSize);
			var remainderList = thisAsList.GetRange(contentListSize, thisAsList.Count - contentListSize);
			return (new StringTokenCollection(contentList), new StringTokenCollection(remainderList));
		}

		/// <summary>
		///   Converts all tokens in this collection into strings.
		/// </summary>
		/// <returns>Collection of strings built from the tokens in this collection.</returns>
		internal IEnumerable<string> GetStrings()
		{
			List<string> results = new List<string>();
			var currentString = new StringBuilder();

			void CompleteCurrentLine()
			{
				results.Add(currentString.ToString());
				currentString = new StringBuilder();
			}

			foreach (var token in _tokens)
				if (token.Type == StringToken.TokenType.LineBreak)
					CompleteCurrentLine();
				else
					currentString.Append(token.String);
			CompleteCurrentLine();

			return results;
		}

		/// <summary>
		///   Concatenates all tokens in this collection into a single string.
		/// </summary>
		/// <returns>A string created by concatenating all tokens in this collection.</returns>
		public override string ToString()
		{
			return string.Concat(_tokens.Select(token => token.String));
		}
	}
}