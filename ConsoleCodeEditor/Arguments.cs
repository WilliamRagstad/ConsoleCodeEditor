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
                    string key = args[i].Replace(keySelector.ToString(), "");
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
                    if (!arguments.Dictionary.ContainsKey(key)) arguments.Dictionary.Add(key, values);
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
        public int Length => Dictionary.Count;

        public bool FindPattern(string key, params Type[] types)
        {
            if (!Dictionary.ContainsKey(key)) return false;
            List<string> keyValues = Dictionary[key];
            for (int i = 0; i < keyValues.Count; i++)
            {
                for (int j = 0; j < types.Length; j++)
                {
                    if (types[j] == typeof(string)) continue; // A string is always convertable to a string.

                    object typeVal = null;
                    try
                    {
                        typeVal = Convert.ChangeType(keyValues[i], types[j]);
                    }
                    catch { }
                    if (typeVal == null) return false;
                }
            }
            return true;
        }
    }
}
