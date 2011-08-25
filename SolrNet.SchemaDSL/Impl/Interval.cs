#region SchemaDSL license
// Copyright (c) 2007-2010 Mauricio Scheffer
// Copyright 2011 Matej Skubic - Studio Pešec d.o.o. - SchemaDSL
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion SchemaDSL license

using System;

#pragma warning disable 1591

namespace SolrNet.SchemaDSL.Impl
{
    /// <summary>
    /// Converts the Interval type to and from string.
    /// </summary>
    public class IntervalConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Gets or sets the underlying type.
        /// </summary>
        public Type UnderlyingType { get; set; }

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType() == typeof(string))
            {
                return destinationType.GetMethod("Parse").Invoke(null, new object[] { value.ToString() });
            }
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            if (value.GetType() == typeof(string)) {
                return UnderlyingType.GetMethod("Parse").Invoke(null, new object[] {value.ToString()});
            }
            return null;
            //return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType is IInterval)
            {
                return true;
            }
            return false;
            //return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        {
            // will return true, and pretend everything received is a string
            return true;
        }
    }

    public class IntervalTypeDescriptorProvider : System.ComponentModel.TypeDescriptionProvider
    {
        public override System.ComponentModel.ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new IntervalTypeDescriptor(objectType);
        }
    }

    public class IntervalTypeDescriptor : System.ComponentModel.CustomTypeDescriptor
    {
        private readonly Type conversionType;
        public IntervalTypeDescriptor(Type conversionType)
        {
            this.conversionType = conversionType;
        }

        public override System.ComponentModel.TypeConverter GetConverter()
        {
            if (conversionType == null)
            {
                throw new Exception("Cannot convert to unknown type.");
            }

            IntervalConverter ic = new IntervalConverter {UnderlyingType = conversionType};

            return ic;
        }
    }

    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    [System.ComponentModel.TypeDescriptionProvider(typeof(IntervalTypeDescriptorProvider))]
    public class Interval<T> : IInterval, IComparable<Interval<T>>
    {

        private readonly Border min = new Border();
        private readonly Border max = new Border();

        public Border Min
        {
            get { return min; }
        }
        public Border Max
        {
            get { return max; }
        }

        #region IInterval Members

        object IInterval.Min
        {
            get { return this.Min.Value; }
            set { this.Min.Value = (T)value; }
        }

        object IInterval.Max
        {
            get { return this.Max.Value; }
            set { this.Max.Value = (T)value; }
        }

        #endregion

        public static Interval<T> Create(T min, IntervalBorderType minType, T max, IntervalBorderType maxType)
        {
            Interval<T> instance = new Interval<T>();

            instance.Min.Value = min;
            instance.Min.Type = minType;
            instance.Max.Value = max;
            instance.Max.Type = maxType;

            return instance;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1},{2}{3}", this.min.Type == IntervalBorderType.Inclusive ? "[" : "(", this.min.Value, this.max.Value, this.max.Type == IntervalBorderType.Inclusive ? "]" : ")");
        }

        public static explicit operator string(Interval<T> instance)
        {
            return instance.ToString();
        }

        public static explicit operator Interval<T>(string str)
        {
            return Parse(str);
        }

        public static Interval<T> Parse(string str)
        {
            str = str.Trim();

            var regex = new System.Text.RegularExpressions.Regex("(?<minb>[\\(\\[\\]])(?<min>[^,]*?),(?<max>[^,]*?)(?<maxb>[\\]\\[\\)])", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

            if (!regex.IsMatch(str))
            {
                throw new ArgumentException("Cannot parse interval.", str);
            }

            var instance = new Interval<T>();

            var groups = regex.Match(str).Groups;

            string sMinb = groups[regex.GroupNumberFromName("minb")].Value;
            string sMin = groups[regex.GroupNumberFromName("min")].Value;
            string sMaxb = groups[regex.GroupNumberFromName("maxb")].Value;
            string sMax = groups[regex.GroupNumberFromName("max")].Value;

            switch (sMinb)
            {
                case "]":
                case "(":
                    instance.Min.Type = IntervalBorderType.Exclusive;
                    break;
                case "[":
                    instance.Min.Type = IntervalBorderType.Inclusive;
                    break;
            }

            switch (sMaxb)
            {
                case "[":
                case ")":
                    instance.Max.Type = IntervalBorderType.Exclusive;
                    break;
                case "]":
                    instance.Max.Type = IntervalBorderType.Inclusive;
                    break;
            }

// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
            instance.Min.Value = (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, sMin);
            instance.Max.Value = (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, sMax);
// ReSharper restore AssignNullToNotNullAttribute
// ReSharper restore PossibleNullReferenceException

            return instance;
        }

        [System.Diagnostics.DebuggerDisplay("{ToString()}")]
        public class Border
        {
            public T Value { get; set; }

            public IntervalBorderType Type { get; set; }

            public override string ToString()
            {
                return string.Format("{0}_{1}", Value, Type);
            }
        }


        #region IComparable<Interval<T>> Members

        public int CompareTo(Interval<T> other)
        {
            var thisMin = this.Min.Value as IComparable;

            if (thisMin != null)
            {
                return thisMin.CompareTo(other.Min.Value);
            }

            return 0;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return (this.CompareTo((Interval<T>)obj));
        }

        #endregion
    }

    public enum IntervalBorderType
    {
        Inclusive,
        Exclusive,
    }
}
#pragma warning restore 1591
