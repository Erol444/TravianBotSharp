using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TbsApi.Services
{
    public class Comparator
    {
        /// <summary>
        /// Compares two objects and returns the list of changes
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="oldObject">Old object</param>
        /// <param name="newObject">New object</param>
        /// <returns></returns>
        public List<Changes> Compare<T>(T oldObject, T newObject)
        {
            var ret = new List<Changes>();
            var fields = oldObject.GetType().GetFields();

            foreach(var field in fields)
            {
                var change = new Changes()
                {
                    Property = field.Name,
                    NewValue = field.GetValue(newObject)
                };
                var oldValue = field.GetValue(oldObject);

                if (!Equals(oldValue, change.NewValue))
                {
                    ret.Add(change);
                }
            }
            return ret;
        }
        public class Changes
        {
            public string Property { get; set; }
            public object NewValue { get; set; }
        }
    }
}
