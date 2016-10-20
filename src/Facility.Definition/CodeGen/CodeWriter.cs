﻿using System;
using System.IO;

namespace Facility.Definition.CodeGen
{
	/// <summary>
	/// Helper class for generating code.
	/// </summary>
	public sealed class CodeWriter
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		public CodeWriter(TextWriter textWriter)
		{
			m_textWriter = textWriter;
			m_isNewLine = true;

			IndentText = "\t";
			BlockBeforeText = "{";
			BlockAfterText = "}";
		}

		/// <summary>
		/// The text used for each indent level (default "\t")
		/// </summary>
		public string IndentText { get; set; }

		/// <summary>
		/// The text written on the line before a block (default "{", null for none).
		/// </summary>
		public string BlockBeforeText { get; set; }

		/// <summary>
		/// The text written on the line after a block (default "}", null for none).
		/// </summary>
		public string BlockAfterText { get; set; }

		/// <summary>
		/// Indents lines written within the scope.
		/// </summary>
		public IDisposable Indent()
		{
			bool wasWriteLineSkipped = m_wasWriteLineSkipped;
			m_wasWriteLineSkipped = false;
			m_indentDepth += 1;

			return new Scope(() =>
			{
				m_indentDepth -= 1;
				m_wasWriteLineSkipped = wasWriteLineSkipped;
			});
		}

		/// <summary>
		/// Writes a line of text before and after the indented scope.
		/// </summary>
		public IDisposable Block()
		{
			return Block(BlockBeforeText, BlockAfterText);
		}

		/// <summary>
		/// Writes a line of text before and after the indented scope.
		/// </summary>
		public IDisposable Block(string before)
		{
			return Block(before, BlockAfterText);
		}

		/// <summary>
		/// Writes a line of text before and after the indented scope.
		/// </summary>
		public IDisposable Block(string before, string after)
		{
			if (before != null)
				WriteLine(before);
			IDisposable indent = Indent();
			return new Scope(() =>
			{
				indent.Dispose();
				if (after != null)
					WriteLine(after);
			});
		}

		/// <summary>
		/// Writes the specified text.
		/// </summary>
		public void Write(string text)
		{
			WriteIndent();
			m_textWriter.Write(text);
		}

		/// <summary>
		/// Writes a new line.
		/// </summary>
		public void WriteLine()
		{
			m_textWriter.WriteLine();
			m_isNewLine = true;
		}

		/// <summary>
		/// Writes the specified text followed by a new line.
		/// </summary>
		public void WriteLine(string text)
		{
			Write(text);
			WriteLine();
		}

		/// <summary>
		/// Writes a new line if it has already been called once in this indent scope.
		/// </summary>
		public void WriteLineSkipOnce()
		{
			if (m_wasWriteLineSkipped)
				WriteLine();
			else
				m_wasWriteLineSkipped = true;
		}

		private void WriteIndent()
		{
			if (m_isNewLine)
			{
				for (int i = 0; i < m_indentDepth; i++)
					m_textWriter.Write(IndentText);
				m_isNewLine = false;
			}
		}

		private sealed class Scope : IDisposable
		{
			public Scope(Action action)
			{
				m_action = action;
			}

			public void Dispose()
			{
				if (m_action != null)
				{
					m_action();
					m_action = null;
				}
			}

			Action m_action;
		}

		readonly TextWriter m_textWriter;
		int m_indentDepth;
		bool m_isNewLine;
		bool m_wasWriteLineSkipped;
	}
}