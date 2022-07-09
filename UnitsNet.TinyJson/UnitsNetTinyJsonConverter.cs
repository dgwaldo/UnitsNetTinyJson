// Licensed under MIT No Attribution, see LICENSE file at the root.
// Copyright 2013 Andreas Gullberg Larsen (andreas.larsen84@gmail.com). 
// Modifications: Don Waldo (whereswaldo85@proton.me)

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnitsNet.TinyJson
{
    /// <inheritdoc />
    /// <summary>
    ///     A JSON.net <see cref="T:Newtonsoft.Json.JsonConverter" /> for converting to/from JSON and Units.NET
    ///     units like <see cref="T:UnitsNet.Length" /> and <see cref="T:UnitsNet.Mass" />.
    ///     Creates a compact unit representation when serialized. Value|Unit such as 100.23|ft. 
    /// </summary>
    /// <remarks>
    ///     Relies on reflection and the type names and namespaces as of 3.x.x of Units.NET.
    ///     Assumptions by reflection code in the converter:
    ///     * Unit classes are of type UnitsNet.Length etc.
    ///     * Unit enums are of type UnitsNet.Units.LengthUnit etc.
    ///     * Unit class has a BaseUnit property returning the base unit, such as LengthUnit.Meter
    /// </remarks>
    public class UnitsNetTinyJsonConverter : JsonConverter
    {
        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        ///     The object value.
        /// </returns>
        /// <exception cref="UnitsNetException">Unable to parse value and unit from JSON.</exception>
        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var obj = TryDeserializeAll(reader, objectType);

            if (obj is Array values)
            {
                var elementType = objectType.GetGenericArguments().First();
                // Create array with the requested type, such as `Length[]` or `Frequency[]` or multi-dimensional arrays like `Length[,]` or `Frequency[,,]` 
                var arrayOfQuantities = Array.CreateInstance(elementType, MultiDimensionalArrayHelpers.LastIndex(values));

                // Fill array with parsed quantities
                int[] ind = MultiDimensionalArrayHelpers.FirstIndex(values);
                while (ind != null)
                {
                    arrayOfQuantities.SetValue(values.GetValue(ind), ind);
                    ind = MultiDimensionalArrayHelpers.NextIndex(arrayOfQuantities, ind);
                }
                return arrayOfQuantities;
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The value to write.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="UnitsNetException">Can't serialize 'null' value.</exception>
        public override void WriteJson(JsonWriter writer, object obj, JsonSerializer serializer)
        {

            Type unitsObj = obj.GetType();

            if (obj is Array values)
            {

                var results = Array.CreateInstance(typeof(string), MultiDimensionalArrayHelpers.LastIndex(values));
                var ind = MultiDimensionalArrayHelpers.FirstIndex(values);

                while (ind != null)
                {
                    var quantity = (values.GetValue(ind) as IQuantity);
                    var method = quantity.GetType().GetMethods().Where(x => x.Name == "GetAbbreviation").ToList();
                    var abbrv = method.Last().Invoke(null, new[] { (quantity as dynamic).Unit, null }) as string;

                    results.SetValue(ToCompactUnitString(quantity.Value, abbrv), ind);
                    ind = MultiDimensionalArrayHelpers.NextIndex(results, ind);
                }

                serializer.Serialize(writer, results);
            }
            else if (obj is IQuantity quantity)
            {
                var method = unitsObj.GetMethods().Where(x => x.Name == "GetAbbreviation").ToList();
                var abbrv = method.Last().Invoke(null, new[] { (obj as dynamic).Unit, null }) as string;
                serializer.Serialize(writer, ToCompactUnitString(quantity.Value, abbrv));
            }
            else
            {
                //return obj;
                //throw new NotSupportedException($"Unsupported type: {obj.GetType()}");
            }
        }

        private static object? TryDeserializeAll(JsonReader reader, Type objectType)
        {
            JToken token = JToken.Load(reader);

            if (IsNullOrEmpty(token))
            {
                return null;
            }

            if (token is JArray)
            {
                var elementType = objectType.GetGenericArguments().First();
                object[] results = token.Children().Select(item => TryDeserializeToUnit(item, elementType)).ToArray();
                return results;
            }
            else
            {
                return TryDeserializeToUnit(token, objectType);
            }
        }

        private static object TryDeserializeToUnit(JToken token, Type objectType)
        {
            if (!token.HasValues)
            {
                var qntyVal = token.ToString().Split("|");
                var quantity = double.Parse(qntyVal[0]);
                var unitType = qntyVal[1];

                var methods = objectType.GetMethods().Where(m => m.Name == "ParseUnit");

                Enum? unitEnum = methods.First().Invoke(null, new[] { unitType }) as Enum;

                return Quantity.From(quantity, unitEnum);
            }
            else
            {
                return null;
            }
        }

        private static string ToCompactUnitString(double value, string unitAbbreviation)
        {
            return $"{Math.Round(value, 4)}|{unitAbbreviation}";
        }

        private static bool IsNullOrEmpty(JToken token)
        {
            var isEmpty = token.ToString() == "{}"
                || token.ToString() == "[]"
                || token.ToString() == String.Empty;
            return (token == null) ||
                   (token.Type == JTokenType.Array && isEmpty) ||
                   (token.Type == JTokenType.Object && isEmpty) ||
                   (token.Type == JTokenType.String && isEmpty) ||
                   (token.Type == JTokenType.Null);
        }

        #region Can Convert

        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            if (IsNullable(objectType))
            {
                return CanConvertNullable(objectType);
            }

            var canConvert = objectType.Namespace != null &&
                  (objectType.Namespace.Equals(nameof(UnitsNet)) ||
                  // All unit types implement IComparable
                  objectType == typeof(IComparable));

            return canConvert;
        }

        /// <summary>
        ///     Determines whether the specified object type is actually a <see cref="System.Nullable" /> type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if the object type is nullable; otherwise <c>false</c>.</returns>
        private static bool IsNullable(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        ///     Determines whether this instance can convert the specified nullable object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if the object type is a nullable container for a UnitsNet type; otherwise <c>false</c>.</returns>
        protected virtual bool CanConvertNullable(Type objectType)
        {
            // Need to look at the FullName in order to determine if the nullable type contains a UnitsNet type.
            // For example: FullName = 'System.Nullable`1[[UnitsNet.Frequency, UnitsNet, Version=3.19.0.0, Culture=neutral, PublicKeyToken=null]]'
            return objectType.FullName != null && objectType.FullName.Contains(nameof(UnitsNet) + ".");
        }

        #endregion

    }


    /// <summary>
    ///     Helper class for working with and manipulating multi-dimension arrays based on their generic index.
    /// </summary>
    internal static class MultiDimensionalArrayHelpers
    {

        /// <summary>
        /// Returns a new array of same Rank and Length as <paramref name="array"/> but with each element converted to <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Array ConvertArrayElements<TResult>(Array array)
        {
            var ret = Array.CreateInstance(typeof(TResult), LastIndex(array));
            var ind = FirstIndex(array);

            while (ind != null)
            {
                ret.SetValue((TResult)array.GetValue(ind), ind);
                ind = NextIndex(array, ind);
            }
            return ret;
        }

        /// <summary>
        /// Returns the index for the 'first' element in a multidimensional array.
        ///
        /// 'First' is defined as the <see cref="Array.GetLowerBound(int)"/> for each rank of the <paramref name="array"/>
        ///
        /// E.g., for a zero-based 5x5x5 array this method would return [0, 0, 0].
        /// </summary>
        /// <param name="array"></param>
        /// <returns>1D integer array specifying the location of the first element in the multidimensional array</returns>
        public static int[] FirstIndex(Array array)
        {
            return Enumerable.Range(0, array.Rank).Select(x => array.GetLowerBound(x)).ToArray();
        }

        /// <summary>
        /// Returns the index for the 'last' element in a multidimensional array.
        ///
        /// 'Last' is defined as the <see cref="Array.GetUpperBound(int)"/> for each rank of the <paramref name="array"/>
        ///
        /// E.g., for a zero-based 5x5x5 array this method would return [4, 4, 4].
        /// </summary>
        /// <param name="array"></param>
        /// <returns>1D integer array specifying the location of the last element in the multidimensional array</returns>
        public static int[] LastIndex(Array array)
        {
            return Enumerable.Range(0, array.Rank).Select(x => array.GetUpperBound(x) + 1).ToArray();
        }

        /// <summary>
        /// Returns the 'next' index after the specified multidimensional <paramref name="index"/>
        ///
        /// The 'next' index is determined by first looping through all elements in the first dimension of the array, then moving on to the next dimension and repeating        
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns>Returns the index location of the next element in <paramref name="array"/> after <paramref name="index"/> as a 1D array of integers.  If there is no next index, returns null</returns>
        public static int[] NextIndex(Array array, int[] index)
        {
            for (var i = 0; i < index.Length; i++)
            {
                index[i] += 1;

                if (index[i] <= array.GetUpperBound(i))
                {
                    return index;
                }
                else
                {
                    index[i] = array.GetLowerBound(i);
                }
            }
            return null;
        }

    }

}