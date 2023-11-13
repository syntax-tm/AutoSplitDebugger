using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSplitDebugger.Core.AutoSplit.Schema;
using PCRE;

namespace AutoSplitDebugger.Core.AutoSplit;

public static class AutoSplitHelper
{
    private const string SECTIONS_REGEX = @"(?<name>state|init|startup|shutdown|update|start|split|isLoading|gameTime|reset|exit)(?<stateDescriptor>\(""(?<module>.+)"".*(?:""(?<version>.+)"")?\))?\s*(?<body>\{(?:[^{}]+|(?5))*+\})";

    private static readonly PcreRegex regex;

    static AutoSplitHelper()
    {
        var settings = new PcreRegexSettings
        {
            Options = PcreOptions.MultiLine | PcreOptions.Extended
        };

        regex = new PcreRegex(SECTIONS_REGEX, settings);
    }

    public static AutoSplitter Parse(string input)
    {


        return default;
    }

}
