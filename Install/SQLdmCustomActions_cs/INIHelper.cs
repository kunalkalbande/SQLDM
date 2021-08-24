using System;
using System.Collections.Generic;
using System.Text;

//Added in SQL DM 9.0 (Vineet Kumar) -- Ini Helpers for writing ini files.
namespace CustomActions
{
    //Represents a key value member of a section
    public class IniKeyValuePair
    {
        public IniKeyValuePair(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return this.Key + " = " + this.Value;
        }
    }

    //Represents a section of inifile
    public class IniSection
    {

        public IniSection()
            : this(String.Empty)
        {

        }
        public IniSection(string name)
        {
            this.SectionName = name;
            this.Members = new List<IniKeyValuePair>();
        }
        public string SectionName { get; set; }
        public List<IniKeyValuePair> Members { get; set; }
        public override string ToString()
        {
            string section = "[" + this.SectionName + "]";
            section += Environment.NewLine;
            foreach (IniKeyValuePair key in this.Members)
            {
                section += key.ToString() + Environment.NewLine;
            }
            return section;
        }
    }

    //Represents the ini complete file
    public class IniFile
    {
        public IniFile(string fileName)
        {
            this.FileName = fileName;
            this.Sections = new List<IniSection>();
        }
        public IniFile()
            : this(String.Empty)
        {
        }

        public string FileName { get; set; }
        public List<IniSection> Sections { get; set; }
        public override string ToString()
        {
            string iniFile = string.Empty;
            foreach (IniSection section in this.Sections)
            {
                iniFile += section.ToString() + Environment.NewLine;
            }
            return iniFile;
        }
    }
}