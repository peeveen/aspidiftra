namespace Aspidiftra
{
	/// <summary>
	///   A string token. Text watermarks will have their text split up into tokens
	///   so that wrapping can be done easier. A token can be either text, whitespace,
	///   or a line break.
	/// </summary>
	internal class StringToken
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="str">The string in the token.</param>
		/// <param name="type">The type of token.</param>
		internal StringToken(string str, TokenType type)
		{
			String = str;
			Type = type;
		}

		/// <summary>
		///   The string content of this token.
		/// </summary>
		internal string String { get; }

		/// <summary>
		///   The type of this token.
		/// </summary>
		internal TokenType Type { get; }

		/// <summary>
		///   Returns the <see cref="TokenType" /> that is suitable for the given character.
		///   If <paramref name="c" /> is whitespace, you will get <see cref="TokenType.Whitespace" />.
		///   If <paramref name="c" /> is a newline character, you will get <see cref="TokenType.LineBreak" />.
		///   Any other character will return <see cref="TokenType.Text" />.
		/// </summary>
		/// <param name="c">Character to check.</param>
		/// <returns>The token type that matches <paramref name="c" />.</returns>
		internal static TokenType GetTokenTypeForChar(char c)
		{
			if (c == '\n')
				return TokenType.LineBreak;
			return char.IsWhiteSpace(c) ? TokenType.Whitespace : TokenType.Text;
		}

		/// <summary>
		///   Type of string token.
		/// </summary>
		internal enum TokenType
		{
			/// <summary>
			///   A string of entirely non-whitespace characters.
			/// </summary>
			Text,

			/// <summary>
			///   A string of entirely whitespace characters.
			/// </summary>
			Whitespace,

			/// <summary>
			///   A line break.
			/// </summary>
			LineBreak
		}
	}
}