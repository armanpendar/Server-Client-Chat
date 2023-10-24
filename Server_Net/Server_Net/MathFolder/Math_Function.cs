using System;
using System.Collections.Generic;
using System.Text;

namespace Server_Net.MathFolder
{
    static class Math_Function
    {
        public static double Evaluate(String input)
        {
            String expr = "(" + input + ")";
            Stack<String> ops = new Stack<String>();
            Stack<Double> vals = new Stack<Double>();

            bool check = false;
            string tmpSTR = "";
            for (int i = 0; i < expr.Length; i++)
            {
                String s = expr.Substring(i, 1);
                
                if (s.Equals("(")) { }
                else if (s.Equals("+")) { ops.Push(s); check = true;
                    vals.Push(Double.Parse(tmpSTR));
                    tmpSTR = "";
                    check = false;
                }
                else if (s.Equals("-")) { ops.Push(s); check = true;
                    vals.Push(Double.Parse(tmpSTR));
                    tmpSTR = "";
                    check = false;
                }
                else if (s.Equals("*")) { ops.Push(s); check = true;
                    vals.Push(Double.Parse(tmpSTR));
                    tmpSTR = "";
                    check = false;
                }
                else if (s.Equals("/")) { ops.Push(s); check = true;
                    vals.Push(Double.Parse(tmpSTR));
                    tmpSTR = "";
                    check = false;
                }
                else if (s.Equals("sqrt")) { ops.Push(s); check = true;
                    vals.Push(Double.Parse(tmpSTR));
                    tmpSTR = "";
                    check = false;
                }                
                else if (s.Equals(")"))
                {
                    vals.Push(Double.Parse(tmpSTR));
                    tmpSTR = "";
                    check = false;
                    int count = ops.Count;
                    while (count > 0)
                    {
                        String op = ops.Pop();
                        double v = vals.Pop();
                        if (op.Equals("+")) v = vals.Pop() + v;
                        else if (op.Equals("-")) v = vals.Pop() - v;
                        else if (op.Equals("*")) v = vals.Pop() * v;
                        else if (op.Equals("/")) v = vals.Pop() / v;
                        else if (op.Equals("sqrt")) v = Math.Sqrt(v);
                        vals.Push(v);

                        count--;
                    }
                }
                else if (check == false)
                {
                    tmpSTR += s;                    
                }
            }
            return vals.Pop();
        }
        
    }
}
