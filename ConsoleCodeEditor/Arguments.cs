using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor
{
    public struct Arguments
    {
        public static Arguments Parse(string[] args, char keySelector = '/')
        {
            Arguments arguments = new Arguments();
            arguments.Dictionary = new Dictionary<string, List<string>>();
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == keySelector)
                {
                    string key = args[i].TrimStart(keySelector);
                    List<string> values = new List<string>();
                    while(i < args.Length - 1)
                    {
                        i++;
                        if (args[i][0] == keySelector)
                        {
                            // continue with next key argument
                            i--;
                            break;
                        }
                        else
                        {
                            values.Add(args[i]);
                        }
                    }
                    arguments.Dictionary.Add(key, values);
                }
            }

            return arguments;
        }

        public Dictionary<string, List<string>> Dictionary;

        public List<string> this[string key]
        {
            get
            {
                return Dictionary[key];
            }
        }
        public bool Contains(string key) => Dictionary.ContainsKey(key);
    }
}
