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
        StartBrakets, EndBrakets, StartParentheses, EndParentheses,
        Hash, Exclamation
    }
    enum TokenList
    {
        Inter, Char, Equal,Pointer,Value,
        PrintInt, PrintChar,InputInt, InputChar,
        ValueChange,PlusOne,MinusOne,
        PlusValue, MinusValue,
        Plus, Minus,
        StartLoop,EndLoop,
        Other,End,Error
    }
    internal class Interpreter
    {
        FileStream fs;
        GetToken TokenGeter;
        long[] Memory = new long[32768];
        bool End = false;
        List<char> Buffer = new List<char>();
        long Pointer = 0;
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
        void RunToken(Token token)
        {
            //포인터 이동
            if (token.TokenType == TokenList.Pointer)
            {
                if (token.Value < 0 && token.Value > 32765)
                {
                    Console.WriteLine($"Erorr: Index Erorr");
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
                else if (token.TokenType == TokenList.Value)
                {
                    Memory[Pointer] = Memory[token.Value];
                }
                else
                {
                    Console.WriteLine($"Erorr: Next Token is Not Value Type");
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
                while (true)
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
        }
        void RunLoop(long Value)
        {
            if (Memory[Pointer] == 0)
            {
                for (Token token1 = TokenGeter.GetCharToken(); token1.TokenType != TokenList.EndLoop; token1 = TokenGeter.GetCharToken()) ;
            }
            else
            {
                while (true)
                {
                    Token token1 = TokenGeter.GetCharToken();
                    if (token1.TokenType == TokenList.EndLoop)
                    {
                        if (Memory[Pointer] > 0)
                        {
                            fs.Position = Value;
                            TokenGeter.GetChar();
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        RunToken(token1);
                        //Console.WriteLine($"{token1.TokenType}");
                    }
                }
            }
        }
        public void RunCode()
        {
            while (true)
            {
                Token token = TokenGeter.GetCharToken();
                
                //루프 오류
                if (token.TokenType == TokenList.EndLoop)
                {
                    Console.WriteLine($"Erorr");
                }
                //에러 처리
                if (token.TokenType == TokenList.Error)
                {
                    break;
                }
                else if (token.TokenType != TokenList.StartLoop)
                {
                    RunToken(token);
                }
                //루프
                else
                {
                    RunLoop(token.Value);
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

                //주석(미구현)
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
            }
        }
    }
}