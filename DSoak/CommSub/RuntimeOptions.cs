#region License
//
// Command Line Library: Program.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CommandLine;
using CommandLine.Text;
#endregion

namespace CommSub
{
    /// <summary>
    /// Run-time options class
    /// 
    /// Instances of this class hold runtime options parsed from command-line arguments and defaulted by properties.
    /// This class uses the Option attributed in CommandLine library.
    /// </summary>
    public abstract class RuntimeOptions
    {
        [Option("registry", MetaValue = "STRING", Required = false, HelpText = "Registry's end point")]
        public string Registry { get; set; }

        [Option("label", MetaValue = "STRING", Required = false, HelpText = "Process label")]
        public string Label { get; set; }

        [Option("anumber", MetaValue = "STRING", Required = false, HelpText = "End-user's A#")]
        public string ANumber { get; set; }

        [Option("firstname", MetaValue = "STRING", Required = false, HelpText = "End-user's first name")]
        public string FirstName { get; set; }

        [Option("lastname", MetaValue = "STRING", Required = false, HelpText = "End-user's last name")]
        public string LastName { get; set; }

        [Option("alias", MetaValue = "STRING", Required = false, HelpText = "End-user's alias")]
        public string Alias { get; set; }

        [Option("minport", MetaValue = "INT", Required = false, HelpText = "Min port in a range of possible ports for this process's communications")]
        public int? MinPortNullable { get; set; }
        public int MinPort { get { return (MinPortNullable==null) ? 0 : (int) MinPortNullable; } }

        [Option("maxport", MetaValue = "INT", Required = false, HelpText = "Max port in a range of possible ports for this process's communications")]
        public int? MaxPortNullable { get; set; }
        public int MaxPort { get { return (MaxPortNullable == null) ? 0 : (int)MaxPortNullable; } }

        [Option("timeout", MetaValue = "INT", Required = false, HelpText = "Default timeout for request-reply communications")]
        public int? TimeoutNullable { get; set; }
        public int Timeout { get { return (TimeoutNullable == null) ? 0 : (int)TimeoutNullable; } }

        [Option('s', "autostart", Required = false, HelpText = "Autostart")]
        public bool AutoStart { get; set; }

        [Option("retries", MetaValue = "INT", Required = false, HelpText = "Default max retries for request-reply communications")]
        public int? RetriesNullable { get; set; }
        public int Retries { get { return (RetriesNullable == null) ? 0 : (int)RetriesNullable; } }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public abstract void SetDefaults();
    }
}
