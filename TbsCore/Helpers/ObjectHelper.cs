﻿using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TbsCore.Models.AccModels;

namespace TbsCore.Helpers
{
    public static class ObjectHelper
    {
        /// <summary>
        /// This method will go through each account object and will add missing objects.
        /// This means users won't need to delete accounts.txt files when there is a new version of
        /// the bot.
        /// </summary>
        /// <param name="acc">Account</param>z
        /// <param name="obj">Object</param>
        public static void FixAccObj(Account acc, object obj)
        {
            if (obj == null) return;
            try
            {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string ns = property.PropertyType.Namespace;
                    if (ns.StartsWith("TravBot") || ns.StartsWith("TbsCore"))
                    {
                        // Class is null, create it and call Init() if it's there
                        if (property.GetValue(obj) == null && property.PropertyType.GetMethod("Init") != null)
                        {
                            // Create missing object
                            object instance = Activator.CreateInstance(property.PropertyType);

                            // Call Init() method of the newly created object.
                            // If it requires acc parameter (only allowed parameter), pass it
                            // -- Currently not possible anyways.
                            var initMethod = property.PropertyType.GetMethod("Init");
                            if (initMethod.GetParameters().Any()) initMethod.Invoke(instance, new object[] { acc });
                            else initMethod.Invoke(instance, null);

                            // Set the missing & populated object to the property
                            property.SetValue(obj, instance);
                        }

                        if (property.GetValue(obj) != null) FixAccObj(acc, property.GetValue(obj));
                    }
                    else if (IsIEnumerable(property.GetValue(obj))) // If object has interface IEnumerable, loop through it's items
                    {
                        if (property.GetValue(obj) != null)
                        {
                            foreach (object item in (IEnumerable)property.GetValue(obj))
                            {
                                FixAccObj(acc, item);
                            }
                        }
                        // else initialize the enumerable?
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ${typeof(ObjectHelper)}", e);
            }
        }

        /// <summary>
        /// Checks if the object implements IEnumerable interface.
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>Whether the object implements IEnumerable interface</returns>
        public static bool IsIEnumerable(object o)
        {
            if (o == null) return false;
            return o is IEnumerable &&
                   o.GetType().IsGenericType;
        }

        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }
    }
}
