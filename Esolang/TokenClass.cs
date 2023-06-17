using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esolang
{
    enum CharList
    {
        Other, Expected,
        Char, Num, Equal, EOF,
        Colon, Semicolon, Comma, Dot, Dollar,
        Plus, Minus,
        Quote, DoubleQuote,
        BackSlash, Slash,
        StartBrakets, EndBrakets, StartParentheses, EndParentheses,
        Hash, Exclamation
    }
    enum TokenList
    {
        Inter, Char, Equal, Pointer, Value,
        PrintInt, PrintChar, InputInt, InputChar,
        ValueChange, PlusOne, MinusOne,
        PlusValue, MinusValue,
        Plus, Minus,
        StartLoop, EndLoop,
        Other, End, Error
    }
    class GetToken
        {
            char Char = ' ';
            public FileStream fs;
            CharList[] charLists = new CharList[256];
            void CharListinit()
            {
                //배열 초기화
                for (int i = 0; i < 256; i++)
                {
                    charLists[i] = CharList.Other;
                }

                //정수 키워드 설정
                for (int i = '0'; i <= '9'; i++)
                {
                    charLists[i] = CharList.Num;
                }

                //문자 키워드 설정
                for (int i = 'a'; i <= 'z'; i++)
                {
                    charLists[i] = CharList.Char;
                }
                for (int i = 'A'; i <= 'Z'; i++)
                {
                    charLists[i] = CharList.Char;
                }
                charLists['_'] = CharList.Char;

                //연산자
                charLists['='] = CharList.Equal;
                charLists['+'] = CharList.Plus;
                charLists['-'] = CharList.Minus;
                charLists['*'] = CharList.Equal;
                charLists['/'] = CharList.Equal;
                charLists['%'] = CharList.Equal;
                charLists['!'] = CharList.Exclamation;

                //따옴표
                charLists['\''] = CharList.Quote;
                charLists['\"'] = CharList.DoubleQuote;

                //슬래쉬
                charLists['\\'] = CharList.BackSlash;
                charLists['/'] = CharList.BackSlash;

                //포인터
                charLists['#'] = CharList.Hash;
                charLists['$'] = CharList.Dollar;

                //입/출력
                charLists['.'] = CharList.Dot;
                charLists[','] = CharList.Comma;
                charLists[':'] = CharList.Colon;
                charLists[';'] = CharList.Semicolon;

                //괄호
                charLists['['] = CharList.StartBrakets;
                charLists[']'] = CharList.EndBrakets;
                charLists['('] = CharList.StartParentheses;
                charLists[')'] = CharList.EndBrakets;

                //예정
                charLists['`'] = CharList.Expected;
                charLists['~'] = CharList.Expected;
                charLists['{'] = CharList.Expected;
                charLists['}'] = CharList.Expected;
                charLists['<'] = CharList.Expected;
                charLists['>'] = CharList.Expected;
                charLists['/'] = CharList.Expected;
                charLists['@'] = CharList.Expected;
                charLists['^'] = CharList.Expected;
                charLists['&'] = CharList.Expected;
                charLists['|'] = CharList.Expected;
            }
            public CharList GetCharType(char Char)
            {
                if ((int)Char < 0 || (int)Char > 256)
                {
                    return CharList.Other;
                }
                return charLists[(int)Char];
            }
            public void GetChar()
            {
                Char = (char)fs.ReadByte();
            }
            public Token GetCharToken()
            {
                long Value = 0;

                //공백 스킵
                if (Char == ' ' || Char == '\n' || Char == '\t' || Char == '\v' || Char == '\f' || Char == '\r')
                    GetChar();

                //상수
                if (GetCharType(Char) == CharList.Num)
                {

                    for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)fs.ReadByte())
                    {
                        Value = Value * 10 + ((int)Char - '0');
                    }
                    return new Token(TokenList.Inter, Value);
                }
                if (GetCharType(Char) == CharList.Quote)
                {
                    GetChar();
                    Value = (int)Char;
                    if (GetCharType(Char) == CharList.BackSlash)
                    {
                        GetChar();

                        switch (Char)
                        {
                            case 'n':
                                Value = '\n';
                                break;
                            case 't':
                                Value = '\t';
                                break;
                            case '\\':
                                Value = '\\';
                                break;
                            default:
                                return new Token(TokenList.Error, 104);
                        }
                    }
                    GetChar();
                    if (GetCharType(Char) != CharList.Quote)
                    {
                        GetChar();
                        return new Token(TokenList.Error, 102);
                    }
                    else
                    {
                        GetChar();
                        return new Token(TokenList.Char, Value);
                    }

                }
                

                //대입/연산
                if (GetCharType(Char) == CharList.Equal)
                {
                    GetChar();
                    return new Token(TokenList.Equal);
                }
                if (GetCharType(Char) == CharList.Exclamation)
                {
                    GetChar();
                    return new Token(TokenList.ValueChange);
                }
                if (GetCharType(Char) == CharList.Plus)
                {
                    GetChar();
                    if (GetCharType(Char) == CharList.Plus)
                    {
                        GetChar();
                        return new Token(TokenList.PlusOne);
                    }
                    else if (GetCharType(Char) == CharList.Equal)
                    {
                        GetChar();
                        return new Token(TokenList.PlusValue);
                    }
                    else
                        return new Token(TokenList.Plus);
                }
                if (GetCharType(Char) == CharList.Minus)
                {
                    GetChar();
                    if (GetCharType(Char) == CharList.Minus)
                    {
                        GetChar();
                        return new Token(TokenList.MinusOne);
                    }
                    else if (GetCharType(Char) == CharList.Equal)
                    {
                        GetChar();
                        return new Token(TokenList.MinusValue);
                    }
                    else
                        return new Token(TokenList.Minus);
                }


                //입/출력
                if (GetCharType(Char) == CharList.Dot)
                {
                    GetChar();
                    return new Token(TokenList.PrintInt);
                }
                if (GetCharType(Char) == CharList.Comma)
                {
                    GetChar();
                    return new Token(TokenList.PrintChar);
                }
                if (GetCharType(Char) == CharList.Colon)
                {
                    GetChar();
                    return new Token(TokenList.InputInt);
                }
                if (GetCharType(Char) == CharList.Semicolon)
                {
                    GetChar();
                    return new Token(TokenList.InputChar);
                }

                //포인터
                if (GetCharType(Char) == CharList.Hash)
                {
                    GetChar();

                    if (GetCharType(Char) != CharList.Num) return new Token(TokenList.Error, 101);
                    for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)fs.ReadByte())
                    {
                        Value = Value * 10 + ((int)Char - '0');
                    }
                    return new Token(TokenList.Pointer, Value);
                }
                if (GetCharType(Char) == CharList.Dollar)
                {
                    GetChar();

                    if (GetCharType(Char) != CharList.Num) return new Token(TokenList.Error, 101);
                    for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)fs.ReadByte())
                    {
                        Value = Value * 10 + ((int)Char - '0');
                    }
                    return new Token(TokenList.Value, Value);
                }

                //루프
                if (GetCharType(Char) == CharList.StartBrakets)
                {
                    GetChar();
                    Value = fs.Position - 1;
                    return new Token(TokenList.StartLoop, Value);
                }
                if (GetCharType(Char) == CharList.EndBrakets)
                {
                    GetChar();
                    return new Token(TokenList.EndLoop);
                }

                //예정
                if (GetCharType(Char) == CharList.DoubleQuote)
                {
                    //Console.WriteLine($"{Char}:DoubleQuote");
                    return new Token(TokenList.Error, 103);
                }
                if (GetCharType(Char) == CharList.Expected)
                {
                    return new Token(TokenList.Error, 103);
                    //Console.WriteLine($"{Char}:DoubleQuote");
                }

                if (GetCharType(Char) == CharList.Char)
                {
                    GetChar();
                }
                if (GetCharType(Char) == CharList.BackSlash)
                {
                    GetChar();
                }
                if (GetCharType(Char) == CharList.Other)
                {
                    GetChar();
                }
                return new Token();
            }
            public GetToken(FileStream s)
            {
                CharListinit();
                fs = s;
            }
        }
        class Token
        {
            public long Value;
            public TokenList TokenType;
            public Token(TokenList tokenType, long value)
            {
                Value = value;
                TokenType = tokenType;
            }
            public Token(TokenList tokenType)
            {
                Value = 0;
                TokenType = tokenType;
            }
            public Token() { Value = 0; TokenType = TokenList.Other; }
        }
    
}
