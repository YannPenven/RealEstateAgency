using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.Tools
{
    public static class ExtensionMethods
    {

        #region ObservableCollection, Dictionary, IEnumerable, Generic

        public static void AddRange<T>(this ObservableCollection<T> destination, IEnumerable<T> items)
        {
            if (items == null) return;
            foreach (T item in items)
            {
                destination.Add(item);
            }
        }

        public static void AddRange<T>(this ObservableCollection<T> destination, ObservableCollection<T> items)
        {
            if (items == null) return;
            foreach (T item in items)
            {
                destination.Add(item);
            }
        }

        public static T Find<T>(this ObservableCollection<T> items, Func<T, bool> predicate)
        {
            if (items == null) return default(T);
            foreach (T item in items)
            {
                if (predicate(item)) return item;
            }
            return default(T);
        }


        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> destination, Dictionary<TKey, TValue> items)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                destination.Add(item.Key, item.Value);
            }
        }


        public static bool Contains<T>(this IEnumerable<T> col, Func<T, bool> predicate)
        {
            foreach (T item in col)
            {
                if (predicate(item)) return true;
            }
            return false;
        }


        public static T[] Add<T>(this T[] target, T item)
        {
            if (target == null)
            {
                return new T[] { item };
            }

            T[] result = new T[target.Length + 1];
            target.CopyTo(result, 0);
            result[target.Length] = item;
            return result;
        }
        public static T[] Insert<T>(this T[] target, int index, T item)
        {
            var myList = new List<T>(target);
            if (myList.Count < index)
            {
                while (myList.Count < index) myList.Add(default(T));
                myList.Add(item);
            }
            else
            {
                myList.Insert(index, item);
            }
            
            return myList.ToArray();
        }

        #endregion


        #region Task

        public static void ExecuteSynchronously(this Task task)
        {
            var ta = task.GetAwaiter();
            ta.GetResult();
        }
        public static T ExecuteSynchronously<T>(this Task<T> task)
        {
            var ta = task.GetAwaiter();
            return ta.GetResult();
        }

        #endregion


        #region Type

        public static bool ImplementInterface(this Type childType, Type interfaceType)
        {
            return childType.GetTypeInfo().ImplementedInterfaces.Contains((Type i) => i == interfaceType);
        }
        public static bool IsChildOf(this Type childType, Type parentType)
        {
            Type parent = childType.GetTypeInfo().BaseType;
            if (parent == null) return false;
            if (parent == parentType) return true;
            return parent.IsChildOf(parentType);
        }
        public static object GetGenericTypeInstance(this Type instanceType, object[] constructorParameters, params Type[] genericTypes)
        {
            Type t = instanceType.MakeGenericType(genericTypes);
            return Activator.CreateInstance(t, constructorParameters);
        }
        public static object ExecuteGenericMethod(this Type methodType, object instance, string methodName, object[] methodParameters, params Type[] genericTypes)
        {
            MethodInfo method = methodType.GetTypeInfo().GetDeclaredMethod(methodName); //methodType.GetRuntimeMethod(methodName, methodSignature);
            MethodInfo genericMethod = method.MakeGenericMethod(genericTypes);
            return genericMethod.Invoke(instance, methodParameters);
        }
        public static async Task ExecuteGenericMethodAsync(this Type methodType, object instance, string methodName, object[] methodParameters, params Type[] genericTypes)
        {
            MethodInfo method = methodType.GetTypeInfo().GetDeclaredMethod(methodName); //methodType.GetRuntimeMethod(methodName, methodSignature);
            MethodInfo genericMethod = method.MakeGenericMethod(genericTypes);
            await (Task)genericMethod.Invoke(instance, methodParameters);
        }
        public static async Task<T> ExecuteGenericMethodAsync<T>(this Type methodType, object instance, string methodName, object[] methodParameters, params Type[] genericTypes)
        {
            MethodInfo method = methodType.GetTypeInfo().GetDeclaredMethod(methodName); //methodType.GetRuntimeMethod(methodName, methodSignature);
            MethodInfo genericMethod = method.MakeGenericMethod(genericTypes);
            return await (Task<T>)genericMethod.Invoke(instance, methodParameters);
        }

        #endregion

    }
}
