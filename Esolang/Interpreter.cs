using System;
using System.Collections.Generic;
using System.IO;
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
        Hash, Exclamation
    }
    enum TokenList
    {
        Inter, Char, Equal,Pointer,Value,
        PrintInt, PrintChar,InputInt, InputChar,
        ValueChange,PlusOne,MinusOne,
        PlusValue, MinusValue,
        Plus, Minus,
        Other,End,Error
    }
    internal class Interpreter
    {
        FileStream fs;
        GetToken TokenGeter;
        int[] Memory = new int[32768];
        bool End = false;
        List<char> Buffer = new List<char>();
        int Pointer = 0;
        public Interpreter(FileStream s)
        {
            fs = s;
            TokenGeter = new GetToken(fs);
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
                    if (token.TokenType == TokenList.Inter || token.TokenType == TokenList.Char )
                    {
                        Memory[Pointer] = token.Value;
                    }
                    else if (token.TokenType == TokenList.Value)
                    {
                        Memory[Pointer] = Memory[token.Value];
                    }
                    else
                    {
                        Console.WriteLine($"Erorr: Next Token is Not Value Type");
                        break;
                    }
                }
                if (token.TokenType == TokenList.PlusValue)
                {
                    token = TokenGeter.GetCharToken();
                    if (token.TokenType == TokenList.Inter || token.TokenType == TokenList.Char)
                    {
                        Memory[Pointer] += token.Value;
                    }
                    else if (token.TokenType == TokenList.Value)
                    {
                        Memory[Pointer] += Memory[token.Value];
                    }
                    else
                    {
                        Console.WriteLine($"Erorr: Next Token is Not Value Type");
                        break;
                    }
                }
                if (token.TokenType == TokenList.MinusValue)
                {
                    token = TokenGeter.GetCharToken();
                    if (token.TokenType == TokenList.Inter || token.TokenType == TokenList.Char)
                    {
                        Memory[Pointer] -= token.Value;
                    }
                    else if (token.TokenType == TokenList.Value)
                    {
                        Memory[Pointer] -= Memory[token.Value];
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

                //연산(외부)
                if (token.TokenType == TokenList.ValueChange)
                {
                    if (Memory[Pointer] == 0)
                    {
                        Memory[Pointer] = 1;
                    }
                    else
                    {
                        Memory[Pointer] = 0;
                    }
                }
                if (token.TokenType == TokenList.PlusOne)
                {
                    Memory[Pointer] += 1;
                }
                if (token.TokenType == TokenList.MinusOne)
                {
                    Memory[Pointer] -= 1;
                }
                //주석
                if (token.TokenType == TokenList.Value)
                {
                    //Console.WriteLine(Memory[token.Value]);
                }
                if (token.TokenType == TokenList.Inter)
                {
                    //Console.WriteLine(Memory[token.Value]);
                }
                if (token.TokenType == TokenList.Char)
                {
                    //Console.WriteLine(Memory[token.Value]);
                }

                //에러 처리
                if (token.TokenType == TokenList.Error)
                {

                    break;
                }
                //프로그램 끝내기
                if (End)
                {
                    break;
                }
                if (fs.Position == fs.Length)
                {
                    End = true;
                }
                
            }
        }
    }
    class GetToken
    {
        char Char = ' ';
        FileStream sr;
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
            charLists['['] = CharList.Semicolon;
            charLists[']'] = CharList.Semicolon;
            charLists['('] = CharList.Semicolon;
            charLists[')'] = CharList.Semicolon;
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
                Char = (char)sr.ReadByte();
            
            //상수
            if (GetCharType(Char) == CharList.Num)
            {

                for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)sr.ReadByte())
                {
                    Value = Value * 10 + ((int)Char - '0');
                }
                return new Token(TokenList.Inter,Value);
            }
            if (GetCharType(Char) == CharList.Quote)
            {
                Char = (char)sr.ReadByte();
                Value = (int)Char;
                if (GetCharType(Char) == CharList.BackSlash)
                {
                    Char = (char)sr.ReadByte();

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
                Char = (char)sr.ReadByte();
                if (GetCharType(Char) != CharList.Quote)
                {
                    Char = (char)sr.ReadByte();
                    return new Token(TokenList.Error, 102);
                }      
                else
                {
                    Char = (char)sr.ReadByte();
                    return new Token(TokenList.Char, Value);
                }
                
            }

            if (GetCharType(Char) == CharList.DoubleQuote)
            {
                //Console.WriteLine($"{Char}:DoubleQuote");
            }

            //대입/연산
            if (GetCharType(Char) == CharList.Equal)
            {
                Char = (char)sr.ReadByte();
                return new Token(TokenList.Equal);
            }
            if (GetCharType(Char) == CharList.Exclamation)
            {
                Char = (char)sr.ReadByte();
                return new Token(TokenList.ValueChange);
            }
            if (GetCharType(Char) == CharList.Plus)
            {
                Char = (char)sr.ReadByte();
                if (GetCharType(Char) == CharList.Plus)
                {
                    Char = (char)sr.ReadByte();
                    return new Token(TokenList.PlusOne);
                }
                else if (GetCharType(Char) == CharList.Equal)
                {
                    Char = (char)sr.ReadByte();
                    return new Token(TokenList.PlusValue);
                }
                else
                    return new Token(TokenList.Plus);
            }
            if (GetCharType(Char) == CharList.Minus)
            {
                Char = (char)sr.ReadByte();
                if (GetCharType(Char) == CharList.Minus)
                {
                    Char = (char)sr.ReadByte();
                    return new Token(TokenList.MinusOne);
                }
                else if (GetCharType(Char) == CharList.Equal)
                {
                    Char = (char)sr.ReadByte();
                    return new Token(TokenList.MinusValue);
                }
                else
                    return new Token(TokenList.Minus);
            }


            //입/출력
            if (GetCharType(Char) == CharList.Dot)
            {
                Char = (char)sr.ReadByte();
                return new Token(TokenList.PrintInt);
            }
            if (GetCharType(Char) == CharList.Comma)
            {
                Char = (char)sr.ReadByte();
                return new Token(TokenList.PrintChar);
            }
            if (GetCharType(Char) == CharList.Colon)
            {
                Char = (char)sr.ReadByte();
                return new Token(TokenList.InputInt);
            }
            if (GetCharType(Char) == CharList.Semicolon)
            {
                Char = (char)sr.ReadByte();
                return new Token(TokenList.InputChar);
            }

            //포인터
            if (GetCharType(Char) == CharList.Hash)
            {
                Char = (char)sr.ReadByte();
                
                if (GetCharType(Char) != CharList.Num) return new Token(TokenList.Error, 101);
                for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)sr.ReadByte())
                {
                    Value = Value * 10 + ((int)Char - '0');
                }
                return new Token(TokenList.Pointer, Value);
            }
            if (GetCharType(Char) == CharList.Dollar)
            {
                Char = (char)sr.ReadByte();

                if (GetCharType(Char) != CharList.Num) return new Token(TokenList.Error, 101);
                for (Value = 0; GetCharType(Char) == CharList.Num; Char = (char)sr.ReadByte())
                {
                    Value = Value * 10 + ((int)Char - '0');
                }
                return new Token(TokenList.Value, Value);
            }

            //루프
            if (GetCharType(Char) == CharList.Char)
            {
                //Console.WriteLine($"{Char}:Char");
            }
            if (GetCharType(Char) == CharList.Char)
            {
                //Console.WriteLine($"{Char}:Char");
            }


            if (GetCharType(Char) == CharList.BackSlash)
            {
                //Console.WriteLine($"{Char}:Backslash");
            }
            if (GetCharType(Char) == CharList.Other)
            {
                //Console.WriteLine($"Hash");
            }
            /*if (sr.EndOfStream)
            {
                //Char = (char)sr.Read();
                return new Token(TokenList.End);
            }*/

            return new Token();

        }
        public GetToken(FileStream s)
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