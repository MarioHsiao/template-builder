﻿namespace LigerShark.TemplateBuilder.Tasks {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    public class TemplateInfo {
        public TemplateInfo() {
            this.Replacements = new Dictionary<string, string>();
        }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public IDictionary<string,string> Replacements { get; set; }

        public static TemplateInfo BuildTemplateInfoFrom(string filePath){
            if (string.IsNullOrEmpty(filePath)) { throw new ArgumentNullException("filePath"); }
            if (!File.Exists(filePath)) { throw new FileNotFoundException("Template info file not found.", filePath); }

            XDocument doc = XDocument.Load(filePath);
            var result = (from r in doc.Root.Elements("Replacements")
                          select new {
                              Include = r.Attribute("Include").Value,
                              Exclude = r.Attribute("Exclude").Value,
                              Replacements = (
                                  from a in r.Elements("add")
                                  select new {
                                      Key = a.Attribute("key").Value,
                                      Value = a.Attribute("value").Value
                                  }
                              )
                          }).Single();

            var tempInfo = new TemplateInfo {
                Include = result.Include,
                Exclude = result.Exclude
            };

            foreach (var r in result.Replacements) {
                tempInfo.Replacements[r.Key] = r.Value;
            }

            return tempInfo;
        }
    }
}
