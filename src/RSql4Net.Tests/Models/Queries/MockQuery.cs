using System;

namespace RSql4Net.Tests.Models.Queries
{
    public class MockQuery
    {
        public AttributeTargets? AttributeTargetsNullP { get; set; }

        public AttributeTargets AttributeTargetsP { get; set; }

        public MockQuery ChildP { get; set; }

        public Guid GuidP { get; set; }
        
        public Guid? GuidNullP { get; set; }
        
        public string StringP { get; set; }

        public short Int16P { get; set; }

        public short? Int16NullP { get; set; }

        public int Int32P { get; set; }

        public int? Int32NullP { get; set; }

        public long Int64P { get; set; }

        public long? Int64NullP { get; set; }

        public bool BooleanP { get; set; }

        public bool? BooleanNullP { get; set; }

        public float SingleP { get; set; }

        public float? SingleNullP { get; set; }

        public decimal DecimalP { get; set; }

        public decimal? DecimalNullP { get; set; }

        public double DoubleP { get; set; }

        public double? DoubleNullP { get; set; }

        public DateTime DateTimeP { get; set; }

        public DateTime? DateTimeNullP { get; set; }

        public DateTimeOffset DateTimeOffsetP { get; set; }

        public DateTimeOffset? DateTimeOffsetNullP { get; set; }

        public char CharP { get; set; }

        public char? CharNullP { get; set; }

        public byte ByteP { get; set; }

        public byte? ByteNullP { get; set; }
    }
}
