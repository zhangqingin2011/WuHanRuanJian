﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HNCAPI
{
    class RandomReader
    {
        private StreamReader _Stream;
        private List<Int32> _LineNos;
        private List<String> _Contents;

        public RandomReader() 
        {
            _LineNos = new List<int>();
            _Contents = new List<string>();

        }
        public bool Open(String filename)
        {
            if (File.Exists(filename) == false) return false;
            _Stream = File.OpenText(filename);
            for (int i = 0; i < 10; i++)
            {
                _LineNos.Add(i);
                _Contents.Add(_Stream.ReadLine());
            }
            return true;
        }
        public void Close() 
        {
            _Stream.Close();
        }
        public String ReadLine(Int32 line)
        {
            if (_LineNos.Contains(line) == false)
            {
                Int32 offset = line - _LineNos[_LineNos.Count - 1];
                for (int i = 0; i < offset+10; i++)
                {
                    if (_Stream.EndOfStream) break;
                    Int32 lineNo = _LineNos[_LineNos.Count - 1]+1;
                    String str = _Stream.ReadLine();
                    _LineNos.Add(lineNo);
                    _Contents.Add(str);
                }
            }
            Int32 index = _LineNos.IndexOf(line);
            if (index != -1) return _Contents[index];
            return "";
        }
    }
}
