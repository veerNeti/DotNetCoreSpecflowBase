using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace DotNetCoreSpecflowBase.Extensions
{
    public static class SpecflowExtensions
    {
        /// <summary>
        /// Usage: string[] expectedLines = table.AsStrings("Line");
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static List<string> GetDataTableAsList(this Table table, string column)
        {
            return table.Rows.Select(row => row[column]).ToList();
        }
    }
}
