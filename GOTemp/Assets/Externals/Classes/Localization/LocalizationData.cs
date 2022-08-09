﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GambitLocalization
{
    public class LocalizationData
    {
        private string langCode = "";
        private List<string> values;

        public string LangCode => langCode;
        public List<string> Values => values;

        public LocalizationData(string langCode)
        {
            this.langCode = langCode;
            values = new List<string>();
        }

        public void AddValue(string v)
        {
            values.Add(v);
        }
    }
}
