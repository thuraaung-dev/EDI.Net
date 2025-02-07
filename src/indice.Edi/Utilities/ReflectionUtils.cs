﻿#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
#if !(NETSTANDARD10 || PORTABLE)
using System.Numerics;
#endif
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Linq;

namespace indice.Edi.Utilities
{
#if (PORTABLE || NETSTANDARD10 || NETSTANDARD13)
    internal enum MemberTypes
    {
        Property = 0,
        Field = 1,
        Event = 2,
        Method = 3,
        Other = 4
    }
    [Flags]
    internal enum BindingFlags
    {
        Default = 0,
        IgnoreCase = 1,
        DeclaredOnly = 2,
        Instance = 4,
        Static = 8,
        Public = 16,
        NonPublic = 32,
        FlattenHierarchy = 64,
        InvokeMethod = 256,
        CreateInstance = 512,
        GetField = 1024,
        SetField = 2048,
        GetProperty = 4096,
        SetProperty = 8192,
        PutDispProperty = 16384,
        ExactBinding = 65536,
        PutRefDispProperty = 32768,
        SuppressChangeType = 131072,
        OptionalParamBinding = 262144,
        IgnoreReturn = 16777216
    }
#endif

    internal static class ReflectionUtils
    {
        public static readonly Type[] EmptyTypes;

        static ReflectionUtils() {
#if !(NETSTANDARD10 || PORTABLE)
            EmptyTypes = Type.EmptyTypes;
#else
            EmptyTypes = new Type[0];
#endif
        }

        public static bool IsVirtual(this PropertyInfo propertyInfo) {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");

            var m = propertyInfo.GetGetMethod();
            if (m != null && m.IsVirtual) {
                return true;
            }

            m = propertyInfo.GetSetMethod();

            if (m != null && m.IsVirtual) {
                return true;
            }

            return false;
        }

        public static MethodInfo GetBaseDefinition(this PropertyInfo propertyInfo) {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");

            var m = propertyInfo.GetGetMethod();
            if (m != null) {
                return m.GetBaseDefinition();
            }

            m = propertyInfo.GetSetMethod();
            if (m != null) {
                return m.GetBaseDefinition();
            }

            return null;
        }

        public static bool IsPublic(PropertyInfo property) {
            if (property.GetGetMethod() != null && property.GetGetMethod().IsPublic) {
                return true;
            }

            if (property.GetSetMethod() != null && property.GetSetMethod().IsPublic) {
                return true;
            }

            return false;
        }

        public static Type GetObjectType(object v) {
            return (v != null) ? v.GetType() : null;
        }


        private static string RemoveAssemblyDetails(string fullyQualifiedTypeName) {
            var builder = new StringBuilder();

            // loop through the type name and filter out qualified assembly details from nested type names
            var writingAssemblyName = false;
            var skippingAssemblyDetails = false;
            for (var i = 0; i < fullyQualifiedTypeName.Length; i++) {
                var current = fullyQualifiedTypeName[i];
                switch (current) {
                    case '[':
                        writingAssemblyName = false;
                        skippingAssemblyDetails = false;
                        builder.Append(current);
                        break;
                    case ']':
                        writingAssemblyName = false;
                        skippingAssemblyDetails = false;
                        builder.Append(current);
                        break;
                    case ',':
                        if (!writingAssemblyName) {
                            writingAssemblyName = true;
                            builder.Append(current);
                        } else {
                            skippingAssemblyDetails = true;
                        }
                        break;
                    default:
                        if (!skippingAssemblyDetails) {
                            builder.Append(current);
                        }

                        break;
                }
            }

            return builder.ToString();
        }

        public static bool HasDefaultConstructor(Type t, bool nonPublic) {
            ValidationUtils.ArgumentNotNull(t, "t");

            if (t.IsValueType()) {
                return true;
            }

            return (GetDefaultConstructor(t, nonPublic) != null);
        }

        public static ConstructorInfo GetDefaultConstructor(Type t) {
            return GetDefaultConstructor(t, false);
        }

        public static ConstructorInfo GetDefaultConstructor(Type t, bool nonPublic) {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (nonPublic) {
                bindingFlags = bindingFlags | BindingFlags.NonPublic;
            }

            return t.GetConstructors(bindingFlags).SingleOrDefault(c => !c.GetParameters().Any());
        }

        public static bool IsNullable(Type t) {
            ValidationUtils.ArgumentNotNull(t, "t");

            if (t.IsValueType()) {
                return IsNullableType(t);
            }

            return true;
        }

        public static bool IsNullableType(Type t) {
            ValidationUtils.ArgumentNotNull(t, "t");

            return (t.IsGenericType() && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type EnsureNotNullableType(Type t) {
            return (IsNullableType(t))
                ? Nullable.GetUnderlyingType(t)
                : t;
        }

        public static bool IsGenericDefinition(Type type, Type genericInterfaceDefinition) {
            if (!type.IsGenericType()) {
                return false;
            }

            var t = type.GetGenericTypeDefinition();
            return (t == genericInterfaceDefinition);
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition) {
            return ImplementsGenericDefinition(type, genericInterfaceDefinition, out var implementingType);
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, out Type implementingType) {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(genericInterfaceDefinition, "genericInterfaceDefinition");

            if (!genericInterfaceDefinition.IsInterface() || !genericInterfaceDefinition.IsGenericTypeDefinition()) {
                throw new ArgumentNullException("'{0}' is not a generic interface definition.".FormatWith(CultureInfo.InvariantCulture, genericInterfaceDefinition));
            }

            if (type.IsInterface()) {
                if (type.IsGenericType()) {
                    var interfaceDefinition = type.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition) {
                        implementingType = type;
                        return true;
                    }
                }
            }

            foreach (var i in type.GetInterfaces()) {
                if (i.IsGenericType()) {
                    var interfaceDefinition = i.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition) {
                        implementingType = i;
                        return true;
                    }
                }
            }

            implementingType = null;
            return false;
        }

        public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition) {
            return InheritsGenericDefinition(type, genericClassDefinition, out var implementingType);
        }

        public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition, out Type implementingType) {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(genericClassDefinition, "genericClassDefinition");

            if (!genericClassDefinition.IsClass() || !genericClassDefinition.IsGenericTypeDefinition()) {
                throw new ArgumentNullException("'{0}' is not a generic class definition.".FormatWith(CultureInfo.InvariantCulture, genericClassDefinition));
            }

            return InheritsGenericDefinitionInternal(type, genericClassDefinition, out implementingType);
        }

        private static bool InheritsGenericDefinitionInternal(Type currentType, Type genericClassDefinition, out Type implementingType) {
            if (currentType.IsGenericType()) {
                var currentGenericClassDefinition = currentType.GetGenericTypeDefinition();

                if (genericClassDefinition == currentGenericClassDefinition) {
                    implementingType = currentType;
                    return true;
                }
            }

            if (currentType.BaseType() == null) {
                implementingType = null;
                return false;
            }

            return InheritsGenericDefinitionInternal(currentType.BaseType(), genericClassDefinition, out implementingType);
        }

        /// <summary>
        /// Gets the type of the typed collection's items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the typed collection's items.</returns>
        public static Type GetCollectionItemType(Type type) {
            ValidationUtils.ArgumentNotNull(type, "type");

            if (type.IsArray) {
                return type.GetElementType();
            }
            if (ImplementsGenericDefinition(type, typeof(IEnumerable<>), out var genericListType)) {
                if (genericListType.IsGenericTypeDefinition()) {
                    throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
                }

                return genericListType.GetGenericArguments()[0];
            }
            if (typeof(IEnumerable).IsAssignableFrom(type)) {
                return null;
            }

            throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
        }

        public static void GetDictionaryKeyValueTypes(Type dictionaryType, out Type keyType, out Type valueType) {
            ValidationUtils.ArgumentNotNull(dictionaryType, "type");

            if (ImplementsGenericDefinition(dictionaryType, typeof(IDictionary<,>), out var genericDictionaryType)) {
                if (genericDictionaryType.IsGenericTypeDefinition()) {
                    throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
                }

                var dictionaryGenericArguments = genericDictionaryType.GetGenericArguments();

                keyType = dictionaryGenericArguments[0];
                valueType = dictionaryGenericArguments[1];
                return;
            }
            if (typeof(IDictionary).IsAssignableFrom(dictionaryType)) {
                keyType = null;
                valueType = null;
                return;
            }

            throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
        }

        /// <summary>
        /// Gets the member's underlying type.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The underlying type of the member.</returns>
        public static Type GetMemberUnderlyingType(MemberInfo member) {
            ValidationUtils.ArgumentNotNull(member, "member");

            switch (member.MemberType()) {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                default:
                    throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo, EventInfo or MethodInfo", nameof(member));
            }
        }

        /// <summary>
        /// Determines whether the member is an indexed property.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        /// 	<c>true</c> if the member is an indexed property; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIndexedProperty(MemberInfo member) {
            ValidationUtils.ArgumentNotNull(member, "member");

            var propertyInfo = member as PropertyInfo;

            if (propertyInfo != null) {
                return IsIndexedProperty(propertyInfo);
            } else {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the property is an indexed property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if the property is an indexed property; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIndexedProperty(PropertyInfo property) {
            ValidationUtils.ArgumentNotNull(property, "property");

            return (property.GetIndexParameters().Length > 0);
        }

        /// <summary>
        /// Gets the member's value on the object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target object.</param>
        /// <returns>The member's value on the object.</returns>
        public static object GetMemberValue(MemberInfo member, object target) {
            ValidationUtils.ArgumentNotNull(member, "member");
            ValidationUtils.ArgumentNotNull(target, "target");

            switch (member.MemberType()) {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(target);
                case MemberTypes.Property:
                    try {
                        return ((PropertyInfo)member).GetValue(target, null);
                    } catch (TargetParameterCountException e) {
                        throw new ArgumentException("MemberInfo '{0}' has index parameters".FormatWith(CultureInfo.InvariantCulture, member.Name), e);
                    }
                default:
                    throw new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture, member.Name), nameof(member));
            }
        }

        /// <summary>
        /// Sets the member's value on the target object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetMemberValue(MemberInfo member, object target, object value) {
            ValidationUtils.ArgumentNotNull(member, "member");
            ValidationUtils.ArgumentNotNull(target, "target");

            switch (member.MemberType()) {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(target, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(target, value, null);
                    break;
                default:
                    throw new ArgumentException("MemberInfo '{0}' must be of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, member.Name), nameof(member));
            }
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be read.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be read.</param>
        /// /// <param name="nonPublic">if set to <c>true</c> then allow the member to be gotten non-publicly.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanReadMemberValue(MemberInfo member, bool nonPublic) {
            switch (member.MemberType()) {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)member;

                    if (nonPublic) {
                        return true;
                    } else if (fieldInfo.IsPublic) {
                        return true;
                    }

                    return false;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)member;

                    if (!propertyInfo.CanRead) {
                        return false;
                    }

                    if (nonPublic) {
                        return true;
                    }

                    return (propertyInfo.GetGetMethod(nonPublic) != null);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be set.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be set.</param>
        /// <param name="nonPublic">if set to <c>true</c> then allow the member to be set non-publicly.</param>
        /// <param name="canSetReadOnly">if set to <c>true</c> then allow the member to be set if read-only.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSetMemberValue(MemberInfo member, bool nonPublic, bool canSetReadOnly) {
            switch (member.MemberType()) {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)member;

                    if (fieldInfo.IsLiteral) {
                        return false;
                    }

                    if (fieldInfo.IsInitOnly && !canSetReadOnly) {
                        return false;
                    }

                    if (nonPublic) {
                        return true;
                    }

                    if (fieldInfo.IsPublic) {
                        return true;
                    }

                    return false;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)member;

                    if (!propertyInfo.CanWrite) {
                        return false;
                    }

                    if (nonPublic) {
                        return true;
                    }

                    return (propertyInfo.GetSetMethod(nonPublic) != null);
                default:
                    return false;
            }
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr) {
            var targetMembers = new List<MemberInfo>();

            targetMembers.AddRange(GetFields(type, bindingAttr));
            targetMembers.AddRange(GetProperties(type, bindingAttr));

            // for some reason .NET returns multiple members when overriding a generic member on a base class
            // http://social.msdn.microsoft.com/Forums/en-US/b5abbfee-e292-4a64-8907-4e3f0fb90cd9/reflection-overriden-abstract-generic-properties?forum=netfxbcl
            // filter members to only return the override on the topmost class
            // update: I think this is fixed in .NET 3.5 SP1 - leave this in for now...
            var distinctMembers = new List<MemberInfo>(targetMembers.Count);

            foreach (var groupedMember in targetMembers.GroupBy(m => m.Name)) {
                var count = groupedMember.Count();
                IList<MemberInfo> members = groupedMember.ToList();

                if (count == 1) {
                    distinctMembers.Add(members.First());
                } else {
                    IList<MemberInfo> resolvedMembers = new List<MemberInfo>();
                    foreach (var memberInfo in members) {
                        // this is a bit hacky
                        // if the hiding property is hiding a base property and it is virtual
                        // then this ensures the derived property gets used
                        if (resolvedMembers.Count == 0) {
                            resolvedMembers.Add(memberInfo);
                        } else if (!IsOverridenGenericMember(memberInfo, bindingAttr) || memberInfo.Name == "Item") {
                            resolvedMembers.Add(memberInfo);
                        }
                    }

                    distinctMembers.AddRange(resolvedMembers);
                }
            }

            return distinctMembers;
        }

        private static bool IsOverridenGenericMember(MemberInfo memberInfo, BindingFlags bindingAttr) {
            if (memberInfo.MemberType() != MemberTypes.Property) {
                return false;
            }

            var propertyInfo = (PropertyInfo)memberInfo;
            if (!IsVirtual(propertyInfo)) {
                return false;
            }

            var declaringType = propertyInfo.DeclaringType;
            if (!declaringType.IsGenericType()) {
                return false;
            }

            var genericTypeDefinition = declaringType.GetGenericTypeDefinition();
            if (genericTypeDefinition == null) {
                return false;
            }

            var members = genericTypeDefinition.GetMember(propertyInfo.Name, bindingAttr);
            if (members.Length == 0) {
                return false;
            }

            var memberUnderlyingType = GetMemberUnderlyingType(members[0]);
            if (!memberUnderlyingType.IsGenericParameter) {
                return false;
            }

            return true;
        }

        public static T GetAttribute<T>(object attributeProvider) where T : Attribute {
            return GetAttribute<T>(attributeProvider, true);
        }

        public static T GetAttribute<T>(object attributeProvider, bool inherit) where T : Attribute {
            var attributes = GetAttributes<T>(attributeProvider, inherit);

            return (attributes != null) ? attributes.FirstOrDefault() : null;
        }

#if !(PORTABLE || NETSTANDARD10 || NETSTANDARD13)
        public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T : Attribute {
            var a = GetAttributes(attributeProvider, typeof(T), inherit);

            if (a is T[]) {
                var attributes = a as T[];
                return attributes;
            }
            return a.Cast<T>().ToArray();
        }

        public static Attribute[] GetAttributes(object attributeProvider, Type attributeType, bool inherit) {
            ValidationUtils.ArgumentNotNull(attributeProvider, "attributeProvider");

            var provider = attributeProvider;

            // http://hyperthink.net/blog/getcustomattributes-gotcha/
            // ICustomAttributeProvider doesn't do inheritance

            if (provider is Type) {
                var t = (Type)provider;
                var a = (attributeType != null) ? t.GetCustomAttributes(attributeType, inherit) : t.GetCustomAttributes(inherit);
                var attributes = a.Cast<Attribute>().ToArray();

#if (NET20 || NET35)
                // ye olde .NET GetCustomAttributes doesn't respect the inherit argument
                if (inherit && t.BaseType != null)
                    attributes = attributes.Union(GetAttributes(t.BaseType, attributeType, inherit)).ToArray();
#endif

                return attributes;
            }

            if (provider is Assembly) {
                var a = (Assembly)provider;
                return (attributeType != null) ? Attribute.GetCustomAttributes(a, attributeType) : Attribute.GetCustomAttributes(a);
            }

            if (provider is MemberInfo) {
                var m = (MemberInfo)provider;
                return (attributeType != null) ? Attribute.GetCustomAttributes(m, attributeType, inherit) : Attribute.GetCustomAttributes(m, inherit);
            }

#if !PORTABLE40
            if (provider is Module) {
                var m = (Module)provider;
                return (attributeType != null) ? Attribute.GetCustomAttributes(m, attributeType, inherit) : Attribute.GetCustomAttributes(m, inherit);
            }
#endif

            if (provider is ParameterInfo) {
                var p = (ParameterInfo)provider;
                return (attributeType != null) ? Attribute.GetCustomAttributes(p, attributeType, inherit) : Attribute.GetCustomAttributes(p, inherit);
            }

#if !PORTABLE40
            var customAttributeProvider = (ICustomAttributeProvider)attributeProvider;
            var result = (attributeType != null) ? customAttributeProvider.GetCustomAttributes(attributeType, inherit) : customAttributeProvider.GetCustomAttributes(inherit);

            return (Attribute[])result;
#else
            throw new Exception("Cannot get attributes from '{0}'.".FormatWith(CultureInfo.InvariantCulture, provider));
#endif
        }
#else
        public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T : Attribute {
            return GetAttributes(attributeProvider, typeof(T), inherit).Cast<T>().ToArray();
        }

        public static Attribute[] GetAttributes(object provider, Type attributeType, bool inherit) {
            if (provider is Type) {
                var t = (Type)provider;
                return (attributeType != null)
                    ? t.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray()
                    : t.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
            }

            if (provider is Assembly) {
                var a = (Assembly)provider;
                return (attributeType != null) ? a.GetCustomAttributes(attributeType).ToArray() : a.GetCustomAttributes().ToArray();
            }

            if (provider is MemberInfo) {
                var m = (MemberInfo)provider;
                return (attributeType != null) ? m.GetCustomAttributes(attributeType, inherit).ToArray() : m.GetCustomAttributes(inherit).ToArray();
            }

            if (provider is Module) {
                var m = (Module)provider;
                return (attributeType != null) ? m.GetCustomAttributes(attributeType).ToArray() : m.GetCustomAttributes().ToArray();
            }

            if (provider is ParameterInfo) {
                var p = (ParameterInfo)provider;
                return (attributeType != null) ? p.GetCustomAttributes(attributeType, inherit).ToArray() : p.GetCustomAttributes(inherit).ToArray();
            }

            throw new Exception("Cannot get attributes from '{0}'.".FormatWith(CultureInfo.InvariantCulture, provider));
        }
#endif

        public static void SplitFullyQualifiedTypeName(string fullyQualifiedTypeName, out string typeName, out string assemblyName) {
            var assemblyDelimiterIndex = GetAssemblyDelimiterIndex(fullyQualifiedTypeName);

            if (assemblyDelimiterIndex != null) {
                typeName = fullyQualifiedTypeName.Substring(0, assemblyDelimiterIndex.Value).Trim();
                assemblyName = fullyQualifiedTypeName.Substring(assemblyDelimiterIndex.Value + 1, fullyQualifiedTypeName.Length - assemblyDelimiterIndex.Value - 1).Trim();
            } else {
                typeName = fullyQualifiedTypeName;
                assemblyName = null;
            }
        }

        private static int? GetAssemblyDelimiterIndex(string fullyQualifiedTypeName) {
            // we need to get the first comma following all surrounded in brackets because of generic types
            // e.g. System.Collections.Generic.Dictionary`2[[System.String, mscorlib,Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
            var scope = 0;
            for (var i = 0; i < fullyQualifiedTypeName.Length; i++) {
                var current = fullyQualifiedTypeName[i];
                switch (current) {
                    case '[':
                        scope++;
                        break;
                    case ']':
                        scope--;
                        break;
                    case ',':
                        if (scope == 0) {
                            return i;
                        }

                        break;
                }
            }

            return null;
        }

        public static MemberInfo GetMemberInfoFromType(Type targetType, MemberInfo memberInfo) {
            const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            switch (memberInfo.MemberType()) {
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)memberInfo;

                    var types = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();

                    return targetType.GetProperty(propertyInfo.Name, bindingAttr, null, propertyInfo.PropertyType, types, null);
                default:
                    return targetType.GetMember(memberInfo.Name, memberInfo.MemberType(), bindingAttr).SingleOrDefault();
            }
        }

        public static IEnumerable<FieldInfo> GetFields(Type targetType, BindingFlags bindingAttr) {
            ValidationUtils.ArgumentNotNull(targetType, "targetType");

            var fieldInfos = new List<MemberInfo>(targetType.GetFields(bindingAttr));
#if !PORTABLE
            // Type.GetFields doesn't return inherited private fields
            // manually find private fields from base class
            GetChildPrivateFields(fieldInfos, targetType, bindingAttr);
#endif

            return fieldInfos.Cast<FieldInfo>();
        }

#if !PORTABLE
        private static void GetChildPrivateFields(IList<MemberInfo> initialFields, Type targetType, BindingFlags bindingAttr) {
            // fix weirdness with private FieldInfos only being returned for the current Type
            // find base type fields and add them to result
            if ((bindingAttr & BindingFlags.NonPublic) != 0) {
                // modify flags to not search for public fields
                var nonPublicBindingAttr = bindingAttr.RemoveFlag(BindingFlags.Public);

                while ((targetType = targetType.BaseType()) != null) {
                    // filter out protected fields
                    var childPrivateFields =
                        targetType.GetFields(nonPublicBindingAttr).Where(f => f.IsPrivate).Cast<MemberInfo>();

                    initialFields.AddRange(childPrivateFields);
                }
            }
        }
#endif

        public static IEnumerable<PropertyInfo> GetProperties(Type targetType, BindingFlags bindingAttr) {
            ValidationUtils.ArgumentNotNull(targetType, "targetType");

            var propertyInfos = new List<PropertyInfo>(targetType.GetProperties(bindingAttr));

            // GetProperties on an interface doesn't return properties from its interfaces
            if (targetType.IsInterface()) {
                foreach (var i in targetType.GetInterfaces()) {
                    propertyInfos.AddRange(i.GetProperties(bindingAttr));
                }
            }

            GetChildPrivateProperties(propertyInfos, targetType, bindingAttr);

            // a base class private getter/setter will be inaccessable unless the property was gotten from the base class
            for (var i = 0; i < propertyInfos.Count; i++) {
                var member = propertyInfos[i];
                if (member.DeclaringType != targetType) {
                    var declaredMember = (PropertyInfo)GetMemberInfoFromType(member.DeclaringType, member);
                    propertyInfos[i] = declaredMember;
                }
            }

            return propertyInfos;
        }

        public static BindingFlags RemoveFlag(this BindingFlags bindingAttr, BindingFlags flag) {
            return ((bindingAttr & flag) == flag)
                ? bindingAttr ^ flag
                : bindingAttr;
        }

        private static void GetChildPrivateProperties(IList<PropertyInfo> initialProperties, Type targetType, BindingFlags bindingAttr) {
            // fix weirdness with private PropertyInfos only being returned for the current Type
            // find base type properties and add them to result

            // also find base properties that have been hidden by subtype properties with the same name

            while ((targetType = targetType.BaseType()) != null) {
                foreach (var propertyInfo in targetType.GetProperties(bindingAttr)) {
                    var subTypeProperty = propertyInfo;

                    if (!IsPublic(subTypeProperty)) {
                        // have to test on name rather than reference because instances are different
                        // depending on the type that GetProperties was called on
                        var index = initialProperties.IndexOf(p => p.Name == subTypeProperty.Name);
                        if (index == -1) {
                            initialProperties.Add(subTypeProperty);
                        } else {
                            var childProperty = initialProperties[index];
                            // don't replace public child with private base
                            if (!IsPublic(childProperty)) {
                                // replace nonpublic properties for a child, but gotten from
                                // the parent with the one from the child
                                // the property gotten from the child will have access to private getter/setter
                                initialProperties[index] = subTypeProperty;
                            }
                        }
                    } else {
                        if (!subTypeProperty.IsVirtual()) {
                            var index = initialProperties.IndexOf(p => p.Name == subTypeProperty.Name
                                                                       && p.DeclaringType == subTypeProperty.DeclaringType);

                            if (index == -1) {
                                initialProperties.Add(subTypeProperty);
                            }
                        } else {
                            var index = initialProperties.IndexOf(p => p.Name == subTypeProperty.Name
                                                                       && p.IsVirtual()
                                                                       && p.GetBaseDefinition() != null
                                                                       && p.GetBaseDefinition().DeclaringType.IsAssignableFrom(subTypeProperty.DeclaringType));

                            if (index == -1) {
                                initialProperties.Add(subTypeProperty);
                            }
                        }
                    }
                }
            }
        }

#if !NETSTANDARD10
        public static bool IsMethodOverridden(Type currentType, Type methodDeclaringType, string method) {
            var isMethodOverriden = currentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(info =>
                    info.Name == method &&
                    // check that the method overrides the original on DynamicObjectProxy
                    info.DeclaringType != methodDeclaringType
                    && info.GetBaseDefinition().DeclaringType == methodDeclaringType
                );

            return isMethodOverriden;
        }
#endif
        public static object GetDefaultValue(Type type) {
            if (!type.IsValueType()) {
                return null;
            }

            switch (ConvertUtils.GetTypeCode(type)) {
                case PrimitiveTypeCode.Boolean:
                    return false;
                case PrimitiveTypeCode.Char:
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.UInt32:
                    return 0;
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return 0L;
                case PrimitiveTypeCode.Single:
                    return 0f;
                case PrimitiveTypeCode.Double:
                    return 0.0;
                case PrimitiveTypeCode.Decimal:
                    return 0m;
                case PrimitiveTypeCode.DateTime:
                    return new DateTime();
#if !(PORTABLE || NETSTANDARD10)
                case PrimitiveTypeCode.BigInteger:
                    return new BigInteger();
#endif
                case PrimitiveTypeCode.Guid:
                    return new Guid();
#if !NET20
                case PrimitiveTypeCode.DateTimeOffset:
                    return new DateTimeOffset();
#endif
            }

            if (IsNullable(type)) {
                return null;
            }

            // possibly use IL initobj for perf here?
            return Activator.CreateInstance(type);
        }

#if net40
        /// <summary>
        /// Sets the propery value of a given object to the given value.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="obj">The instance that contains the property to update</param>
        /// <param name="value">The object value to set.</param>
        internal static void SetValue(this PropertyInfo property, object obj, object value) {
            property.SetValue(obj, value, null);
        }

        /// <summary>
        /// Returns the property value of the specified object.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="obj">The instance that contains the property to retrieve</param>
        /// <returns>the value of the property</returns>
        internal static object GetValue(this PropertyInfo property, object obj) {
            return property.GetValue(obj, null);
        }

        internal static Type GetTypeInfo(this Type type) {
            return type;
        }

        internal static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element, bool inherit = true) where T : Attribute {
            return element.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }
#endif
    }
}