﻿using Gameloop.Vdf.Linq;

namespace Gameloop.Vdf;

public class VdfTextWriter : VdfWriter
{
    private readonly TextWriter _writer;
    private int _indentationLevel;

    public VdfTextWriter(TextWriter writer) : this(writer, VdfSerializerSettings.Default) { }

    public VdfTextWriter(TextWriter writer, VdfSerializerSettings settings) : base(settings)
    {
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        _writer = writer;
        _indentationLevel = 0;
    }

    public override void WriteKey(string key)
    {
        AutoComplete(State.Key);
        _writer.Write(VdfStructure.Quote);
        WriteEscapedString(key);
        _writer.Write(VdfStructure.Quote);
    }

    public override void WriteValue(VValue value)
    {
        AutoComplete(State.Value);
        _writer.Write(VdfStructure.Quote);
        WriteEscapedString(value.ToString());
        _writer.Write(VdfStructure.Quote);
    }

    public override void WriteObjectStart()
    {
        AutoComplete(State.ObjectStart);
        _writer.Write(VdfStructure.ObjectStart);

        _indentationLevel++;
    }

    public override void WriteObjectEnd()
    {
        _indentationLevel--;

        AutoComplete(State.ObjectEnd);
        _writer.Write(VdfStructure.ObjectEnd);

        if (_indentationLevel == 0)
            AutoComplete(State.Finished);
    }

    public override void WriteComment(string text)
    {
        AutoComplete(State.Comment);
        _writer.Write(VdfStructure.Comment);
        _writer.Write(VdfStructure.Comment);
        _writer.Write(text);
    }

        public override void WriteConditional(IReadOnlyList<VConditional.Token> tokens)
        {
            AutoComplete(State.Conditional);
            _writer.Write(VdfStructure.ConditionalStart);

            foreach (VConditional.Token token in tokens)
            {
                switch (token.TokenType)
                {
                    case VConditional.TokenType.Constant:
                        _writer.Write(VdfStructure.ConditionalConstant);
                        _writer.Write(token.Name);
                        break;

                    case VConditional.TokenType.Not:
                        _writer.Write(VdfStructure.ConditionalNot);
                        break;

                    case VConditional.TokenType.Or:
                        _writer.Write(VdfStructure.ConditionalOr);
                        _writer.Write(VdfStructure.ConditionalOr);
                        break;

                    case VConditional.TokenType.And:
                        _writer.Write(VdfStructure.ConditionalAnd);
                        _writer.Write(VdfStructure.ConditionalAnd);
                        break;
                }
            }

            _writer.Write(VdfStructure.ConditionalEnd);
        }

        private void AutoComplete(State next)
        {
            if (CurrentState == State.Start)
            {
                CurrentState = next;
                return;
            }

            switch (next)
            {
                case State.Value:
                case State.Conditional:
                    _writer.Write(VdfStructure.Assign);
                    break;

            case State.Key:
            case State.ObjectStart:
            case State.ObjectEnd:
            case State.Comment:
                _writer.WriteLine();
                _writer.Write(new string(VdfStructure.Indent, _indentationLevel));
                break;

            case State.Finished:
                _writer.WriteLine();
                break;
        }

        CurrentState = next;
    }

    private void WriteEscapedString(string str)
    {
        if (!Settings.UsesEscapeSequences)
        {
            _writer.Write(str);
            return;
        }

        foreach (char ch in str)
        {
            if (!VdfStructure.IsEscapable(ch))
                _writer.Write(ch);
            else
            {
                _writer.Write(VdfStructure.Escape);
                _writer.Write(VdfStructure.GetEscape(ch));
            }
        }
    }

    public override void Close()
    {
        base.Close();
        if (CloseOutput)
            _writer.Dispose();
    }
}
