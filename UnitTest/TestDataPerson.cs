using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace UnitTest
{

    //small test data class
    public class TestDataPerson
    {
        [LoadColumn(0)]
        public string NameFirst { get; set; } = "";
        [LoadColumn(1)]
        public string NameLast { get; set; } = "";
        [LoadColumn(2)]
        public string Street { get; set; } = "";
        [LoadColumn(3)]
        public string City { get; set; } = "";
        [LoadColumn(4)]
        public float PostalCode { get; set; } = 0.0f;

    }
}
