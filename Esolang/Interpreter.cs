using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esolang
{
    enum CharList
    {
        Other,
        Char, Num, Equal, EOF,
        Colon, Semicolon, Comma, Dot, Dollar,
        Plus,Minus,
        Quote, DoubleQuote,
        BackSlash, Slash,
        Hash
    }
    enum TokenList
    {
        Inter, Char, Equal,Pointer,Value,
        PrintInt, PrintChar,InputInt, InputChar,
        Other,End,Error
    }
    internal class Interpreter
    {
        StreamReader sr;
        GetToken TokenGeter;
        int[] Memory = new int[32768];
        List<char> Buffer = new List<char>();
        int Pointer = 0;
        public Interpreter(StreamReader s)
        {
            sr = s;
            TokenGeter = new GetToken(sr);
        }
        void GetBuffer()
        {
            while (Buffer.Count == 0)
            {
                string temp = Console.ReadLine();
                if (temp == null) { }
                foreach (char c in temp)
                {
                    Buffer.Add(c);
                }
                Buffer.Add('\n');
            }
        }
        void PrintErorr(int ErorrCode)
        {

        }
        public void RunCode()
        {
            while (true)
            {
                Token token = TokenGeter.GetCharToken();
                //포인터 이동
                if (token.TokenType == TokenList.Pointer)
                {
                    if (token.Value < 0 && token.Value > 32765)
                    {
                        Console.WriteLine($"Erorr: Index Erorr");
                        break;
                    }
                    else
                    {
                        Pointer = token.Value;
                    }
                }
                //대입
                if (token.TokenType == TokenList.Equal)
                {
                    token = TokenGeter.GetCharToken();
                    if (token.TokenType == TokenList.Inter || token.TokenType == TokenList.Char)
                    {
                        Memory[Pointer] = token.Value;
                    }
                    else
                    {
                        Console.WriteLine($"Erorr: Next Token is Not Value Type");
                        break;
                    }
                }
                //입/출력
                if (token.TokenType == TokenList.PrintInt)
                {
                    Console.Write(Memory[Pointer]);
                }
                if (token.TokenType == TokenList.PrintChar)
                {
                    Console.Write((char)Memory[Pointer]);
                }
                if (token.TokenType == TokenList.InputInt)
                {
                    int Value = 0;
                    if (Buffer.Count == 0)
                    {
                        GetBuffer();
                    }
                    for (;;)
                    {
                        if (Buffer.Count == 0)
                        {
                            GetBuffer();
                        }
                        if (TokenGeter.GetCharType(Buffer[0]) != CharList.Num)
                        {
                            Buffer.RemoveAt(0);
                            continue;
                        }
                        while (TokenGeter.GetCharType(Buffer[0]) == CharList.Num)
                        {
                            Value = Value * 10 + ((int)Buffer[0] - '0');
                            Buffer.RemoveAt(0);
                        }
                        break;
                    }

                    Memory[Pointer] = Value;
                    Buffer.Remove(Buffer[0]);
                }
                if (token.TokenType == TokenList.InputChar)
                {
                    if (Buffer.Count == 0)
                    {
                        while (Buffer.Count == 0)
                        {
                            GetBuffer();
                        }
                    }
                    Memory[Pointer] = Buffer[0];
                    Buffer.RemoveAt(0);
                }
                if (token.TokenType == TokenList.Value)
                {
                    //Console.WriteLine(Memory[token.Value]);
                }
                if (token.TokenType == TokenList.Char)
                {
                }
                if (token.TokenType == TokenList.Error)
                {

                    break;
                }
                if (token.TokenType == TokenList.End)
                {
                    break;
                }
            }
        }
    }
    class GetToken
    {
        char Char = ' ';
        StreamReader sr;
        CharList[] charLists = new CharList[256];
        void CharListinit()
        {
            for (int i = 0; i < 256; i++)
            {
                charLists[i] = CharList.Other;
            }
            for (int i = '0'; i <= '9'; i++)
            {
                charLists[i] = CharList.Num;
            }
            for (int i = '0'; i <= '9'; i++)
            {
                charLists[i] = CharList.Num;
            }
            for (int i = 'a'; i <= 'z'; i++)
            {
                charLists[i] = CharList.Char;
            }
            for (int i = 'A'; i <= 'Z'; i++)
            {
                charLists[i] = CharList.Char;
            }
            charLists['='] = CharList.Equal;
            charLists['\''] = CharList.Quote;
            charLists['\"'] = CharList.DoubleQuote;
            charLists['\\'] = CharList.BackSlash;
            charLists['#'] = CharList.Hash;
            charLists['$'] = CharList.Dollar;
            charLists[','] = CharList.Comma;
            charLists['.'] = CharList.Dot;
            charLists['+'] = CharList.Plus;
            charLists['-'] = CharList.Minus;
            charLists[':'] = CharList.Colon;
            charLists[';'] = CharList.Semicolon;
        }
        public CharList GetCharType(char Char)
        {
            if ((int)Char < 0 || (int)Char > 256)
            {
                return CharList.Other;
            }
            return charLists[(int)Char];
        }
        public Token GetCharToken()
        {
            int Value = 0;

            //공백 스킵
            if (Char == ' ' || Char == '\n' || Char == '\t' || Char == '\v' || Char == '\f' || Char == '\r')
                Char = (char)sr.Read();
            //상수
            /*정수*/if (GetCharType(Char) == CharList.Num)
            {
                for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)sr.Read())
                {
                    Value = Value * 10 + ((int)Char - '0');
                }
                return new Token(TokenList.Inter,Value);
            }
            /*문자*/if (GetCharType(Char) == CharList.Quote)
            {
                Char = (char)sr.Read();
                Value = (int)Char;
                if (GetCharType(Char) == CharList.BackSlash)
                {
                    Char = (char)sr.Read();

                    switch (Char)
                    {
                        case 'n':
                            Value = '\n';
                            break;
                        case 't':
                            Value = '\t';
                            break;
                    }
                }
                Char = (char)sr.Read();
                if (GetCharType(Char) != CharList.Quote)
                {
                    Char = (char)sr.Read();
                    return new Token(TokenList.Error, 102);
                }      
                else
                {
                    Char = (char)sr.Read();
                    return new Token(TokenList.Char, Value);
                }
                
            }

            //대입
            if (GetCharType(Char) == CharList.Equal)
            {
                Char = (char)sr.Read();
                return new Token(TokenList.Equal);
            }
            if (GetCharType(Char) == CharList.Plus)
            {

            }
            //입/출력
            if (GetCharType(Char) == CharList.Dot)
            {
                Char = (char)sr.Read();
                return new Token(TokenList.PrintInt);
            }
            if (GetCharType(Char) == CharList.Comma)
            {
                Char = (char)sr.Read();
                return new Token(TokenList.PrintChar);
            }
            if (GetCharType(Char) == CharList.Colon)
            {
                Char = (char)sr.Read();
                return new Token(TokenList.InputInt);
            }
            if (GetCharType(Char) == CharList.Semicolon)
            {
                Char = (char)sr.Read();
                return new Token(TokenList.InputChar);
            }
            //포인터
            if (GetCharType(Char) == CharList.Hash)
            {
                Char = (char)sr.Read();
                
                if (GetCharType(Char) != CharList.Num) return new Token(TokenList.Error, 101);
                for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)sr.Read())
                {
                    Value = Value * 10 + ((int)Char - '0');
                }
                return new Token(TokenList.Pointer, Value);
            }
            if (GetCharType(Char) == CharList.Dollar)
            {
                Char = (char)sr.Read();

                if (GetCharType(Char) != CharList.Num) return new Token(TokenList.Error, 101);
                for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)sr.Read())
                {
                    Value = Value * 10 + ((int)Char - '0');
                }
                return new Token(TokenList.Value, Value);
            }
            if (GetCharType(Char) == CharList.Semicolon)
            {
                //Console.WriteLine($"Comma");
            }
            if (GetCharType(Char) == CharList.Char)
            {
                //Console.WriteLine($"{Char}:Char");
            }
            if (GetCharType(Char) == CharList.DoubleQuote)
            {
                //Console.WriteLine($"{Char}:DoubleQuote");
            }
                
            if (GetCharType(Char) == CharList.BackSlash)
            {
                //Console.WriteLine($"{Char}:Backslash");
            }
            if (GetCharType(Char) == CharList.Other)
            {
                //Console.WriteLine($"Hash");
            }
            if (sr.EndOfStream)
            {
                return new Token(TokenList.End);
            }

            return new Token();

        }
        public GetToken(StreamReader s)
        {
            CharListinit();
            sr = s;
        }
    }
    class Token{
        public int Value;
        public TokenList TokenType;
        public Token(TokenList tokenType,int value)
        {
            Value = value;
            TokenType = tokenType;
        }
        public Token(TokenList tokenType)
        {
            Value = 0;
            TokenType = tokenType;
        }
        public Token() { Value = 0; TokenType = TokenList.Other;}
    }
}
//d