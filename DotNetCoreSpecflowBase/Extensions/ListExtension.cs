using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCoreSpecflowBase.Extensions
{
    public static class ListExtension
    {
        public static bool SetwiseEquivalentTo<T> (this List<T> list, List<T> other)
            where T : IEquatable<T>
        {
            if (list.Except(other).Any())
                return false;
            if (other.Except(list).Any())
                return false;
            return true;
        }
        public static string SerializeObjectToString(this object someObject)
        {
            return JsonConvert.SerializeObject(someObject);
        }
    }
}
