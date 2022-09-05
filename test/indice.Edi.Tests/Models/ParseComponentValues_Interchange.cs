﻿using System;
using indice.Edi.Serialization;

namespace indice.Edi.Tests.Models
{
    public class ParseComponentValues_Interchange
    {
        public Message Msg { get; set; }

        [EdiMessage]
        public class Message
        {
            [EdiValue("9(1)", Path = "DTM/0")]
            public int Integer { get; set; }

            [EdiValue("9(8)", Path = "DTM/1", Format = "yyyyMMdd", Description = "DTM02 - Date (Format = CCYYMMDD)")]
            [EdiValue("9(8)", Path = "DTM/2", Format = "HHmmssff", Description = "DTM03 - Time (Format = HHmmssff)")]
            public DateTime DateTime { get; set; }

            [EdiValue("9(1)", Path = "DTM/3")]
            public long Long { get; set; }
        }
    }
}
