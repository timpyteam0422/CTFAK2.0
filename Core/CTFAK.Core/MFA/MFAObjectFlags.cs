﻿using CTFAK.CCN.Chunks;
using CTFAK.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFAK.MFA
{
    public class MFAObjectFlags : ChunkLoader
    {
        public List<ObjectFlag> Items = new List<ObjectFlag>();

        public override void Read(ByteReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Items.Count);
            foreach (var item in Items) item.Write(Writer);
        }
    }
    public class ObjectFlag : ChunkLoader
    {
        public string Name;
        public bool Value;

        public override void Read(ByteReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Name);
            Writer.WriteInt32(0);
            if (Value)
                Writer.WriteInt32(1);
            else
                Writer.WriteInt32(0);
        }
    }
}
