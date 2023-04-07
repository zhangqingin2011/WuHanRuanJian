﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI
{
    class GCodeSender
    {
        private Machine _mac;
        private String _FileName;
        private String _GCodeFile;
        public GCodeSender(Machine mac, String GCodeFile)
        {
            _mac = mac;
            _GCodeFile = GCodeFile;
            _FileName = _mac.ProgPath + "\\" + GCodeFile;
        }
        public void Read()
        {
            System.IO.StreamReader reader = null;
            try
            {
                if (System.IO.File.Exists(_FileName) == false) return;
                if (!IsTextFile()) return;
                String MacSN = _mac.MachineSN;
                if (String.IsNullOrEmpty(MacSN)) return;
                reader = new System.IO.StreamReader(_FileName);
                ServiceStack.Redis.RedisClient client = new ServiceStack.Redis.RedisClient("127.0.0.1", 6379);
                if (client.Ping() == false) return;
                client.Db = _mac.LocalDB;
                using (var trans = client.CreatePipeline())
                {
                    if(client.Exists("GCODE:"+_GCodeFile)==1)
                    {
                        trans.QueueCommand(r => r.Remove("GCODE:"+_GCodeFile));
                        trans.Flush();
                    }
                    while (!reader.EndOfStream)
                    {
                        String GCodetext = reader.ReadLine();
                        trans.QueueCommand(r => r.AddItemToList("GCODE:"+_GCodeFile, GCodetext));

                    }
                    trans.Flush();

                }
               
               // reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("GCodeSender" + e.Message);
            }
            finally
            {
                if(reader!=null)
                {
                    reader.Close();
                }
            }
        }
        private bool IsTextFile()
        {
            bool ret = true;
            System.IO.FileStream stream = new System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            try
            {
                int len = (int)stream.Length;
                byte data;
                int i = 0;
                while (i < len && ret) 
                {
                    data = (byte)stream.ReadByte();
                    ret=(data!=0);
                    i++;
                }
                return ret;


            }
            catch (Exception e)
            {
                Console.WriteLine("IsTextFile" + e.Message);
                return false;
            }
            finally 
            {
                stream.Close();
            }
                    
        
        }
    }
}
