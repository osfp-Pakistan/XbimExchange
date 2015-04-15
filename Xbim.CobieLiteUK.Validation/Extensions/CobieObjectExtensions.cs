﻿using System.Collections.Generic;
using System.Linq;
using Xbim.COBieLiteUK;

namespace Xbim.CobieLiteUK.Validation.Extensions
{
    /// <summary>
    /// Provides extension methods to simplify the execution of validation algorithms on CobieLiteUK AssetTypes.
    /// </summary>
    static internal class CobieObjectExtensions
    {
        public static List<T> GetChildObjects<T>(this CobieObject obj)
        {
            if (obj.GetType() == typeof(AssetType))
                return ((AssetType)obj).Assets as List<T>;
            if (obj.GetType() == typeof (Zone))
            {
                return ((Zone)obj).SpaceObjects.ToList() as List<T>;
            }
                
            return null;
        }

        public static void SetChildObjects<TChild>(this CobieObject obj, List<TChild> newChildrenSet)
        {
            if (obj.GetType() == typeof(AssetType))
            {
                ((AssetType)obj).Assets = newChildrenSet as List<Asset>;
            }
            if (obj.GetType() == typeof(Zone))
            {
                
                ((Zone)obj).Spaces = new List<SpaceKey>();
                foreach (var child in newChildrenSet.OfType<Space>() )
                {
                    // todo: resume from here; we need to ensure that the floor and spaces of the provided spaceKey are avalialale in the report facility


                    ((Zone)obj).Spaces.Add(new SpaceKey() { Name = child.Name} );    
                }
            }
        }

        private const string MatchingCategoriesAttributeName = "DPoWMatchingCategories";
        private const string MatchingCodesAttributeName = "DPoWMatchingCodes";

        private const string RequirementCategoriesAttributeName = "DPoWRequirementCategories";
        private const string RequirementCodesAttributeName = "DPoWRequirementCodes";
        private const string RequirementDescsAttributeName = "DPoWRequirementDescs";

        private const string SubmittedAssetsAttributeName = "DPoWSubmittedAssetsCount";
        private const string ValidAssetsAttributeName = "DPoWValidSubmittedAssetsCount";
        private const string RequirementExternalSystemAttributeName = "RefRequirementExternalSystem";
        private const string RequirementExternalIdAttributeName = "RefRequirementExternalId";
        private const string RequirementNameAttributeName = "RefRequirementName";
        private const string AttributesPropertySetName = "DPoW Attributes";

        static public string GetRequirementExternalSystem(this CobieObject retType)
        {
            return GetStringValue(retType, RequirementExternalSystemAttributeName);
        }

        static public string GetRequirementExternalId(this CobieObject retType)
        {
            return GetStringValue(retType, RequirementExternalIdAttributeName);
        }

        static public string GetRequirementName(this CobieObject retType)
        {
            return GetStringValue(retType, RequirementNameAttributeName);
        }


        static public void SetRequirementExternalSystem(this CobieObject retType, string value)
        {
            if (retType == null)
                return;
            SetStringValue(retType, value, RequirementExternalSystemAttributeName, "ExternalSystem of the requirement group.");
        }

        static public void SetRequirementExternalId(this CobieObject retType, string value)
        {
            if (retType == null)
                return;
            SetStringValue(retType, value, RequirementExternalIdAttributeName, "ExternalId of the requirement group.");
        }

        static public void SetRequirementName(this CobieObject retType, string value)
        {
            if (retType == null)
                return;
            SetStringValue(retType, value, RequirementNameAttributeName, "Name of the requirement group.");
        }

        static public void SetSubmittedAssetsCount(this CobieObject retType, int value)
        {
            if (retType == null)
                return;
            SetIntegerValue(retType, value, SubmittedAssetsAttributeName, "Count of submitted items.");
        }

        static public int GetSubmittedAssetsCount(this CobieObject retType)
        {
            return GetIntegerValue(retType, SubmittedAssetsAttributeName);
        }


        static public void SetValidAssetsCount(this CobieObject retType, int value)
        {
            if (retType == null)
                return;
            SetIntegerValue(retType, value, ValidAssetsAttributeName, "Count of submitted items that satisfy requirements.");
        }

        static public int GetValidAssetsCount(this CobieObject retType)
        {
            return GetIntegerValue(retType, ValidAssetsAttributeName);
        }

        private static string GetStringValue(CobieObject retType, string AttributeName)
        {
            if (retType.Attributes == null)
                return "";

            var existingAttribute =
                retType.Attributes.FirstOrDefault(
                    a => a.Name == AttributeName && a.PropertySetName == AttributesPropertySetName);

            if (existingAttribute == null)
                return "";

            var stringValue = existingAttribute.Value as StringAttributeValue;
            return stringValue == null 
                ? @""
                : stringValue.Value;
        }

        private static int GetIntegerValue(CobieObject retType, string AttributeName)
        {
            if (retType.Attributes == null)
                return 0;

            var existingAttribute =
                retType.Attributes.FirstOrDefault(
                    a => a.Name == AttributeName && a.PropertySetName == AttributesPropertySetName);

            if (existingAttribute == null)
                return 0;

            var integerValue = existingAttribute.Value as IntegerAttributeValue;
            return integerValue == null || !integerValue.Value.HasValue
                ? 0
                : integerValue.Value.Value;
        }

        private static void SetStringValue(CobieObject retType, string value, string propertyName, string propertyDescription)
        {
            if (retType.Attributes == null)
                retType.Attributes = new List<Attribute>();

            var existingAttribute =
                retType.Attributes.FirstOrDefault(
                    a => a.Name == propertyName && a.PropertySetName == AttributesPropertySetName);

            if (existingAttribute != null)
            {
                existingAttribute.Value = new StringAttributeValue() { Value = value };
            }
            else
            {
                var matchingClassAttribute = new Attribute
                {
                    Name = propertyName,
                    PropertySetName = AttributesPropertySetName,
                    Description = propertyDescription,
                    Value = new StringAttributeValue() { Value = value },
                    Categories = new List<Category>() { dpowValidatedAttributeClass }
                };
                retType.Attributes.Add(matchingClassAttribute);
            }
        }

        private static Category dpowValidatedAttributeClass
        {
            get { return new Category() {Classification = @"DPoW", Code = "reference"}; }
        }

        private static void SetIntegerValue(CobieObject retType, int value, string propertyName, string propertyDescription)
        {
            if (retType.Attributes == null)
                retType.Attributes = new List<Attribute>();

            var existingAttribute =
                retType.Attributes.FirstOrDefault(
                    a => a.Name == propertyName && a.PropertySetName == AttributesPropertySetName);

            if (existingAttribute != null)
            {
                existingAttribute.Value = new IntegerAttributeValue() {Value = value};
            }
            else
            {
                var matchingClassAttribute = new Attribute
                {
                    Name = propertyName,
                    PropertySetName = AttributesPropertySetName,
                    Description = propertyDescription,
                    Value = new IntegerAttributeValue() {Value = value},
                    Categories = new List<Category>() { dpowValidatedAttributeClass }
                };
                retType.Attributes.Add(matchingClassAttribute);
            }
        }

        public static IEnumerable<Category> GetMatchingCategories(this CobieObject retType)
        {
            var cls = retType.GetMatchingClassifications().GetEnumerator();
            var codes = retType.GetMatchingCodes().GetEnumerator();

            while (cls.MoveNext() && codes.MoveNext())
            {
                yield return new Category()
                {
                    Classification = cls.Current,
                    Code = codes.Current
                };
            }
        }

        public static IEnumerable<Category> GetRequirementCategories(this CobieObject retType)
        {
            var cls = retType.GetRequirementClassifications().GetEnumerator();
            var codes = retType.GetRequirementCodes().GetEnumerator();
            var descs = retType.GetRequirementDescs().GetEnumerator();

            while (cls.MoveNext() && codes.MoveNext() && descs.MoveNext())
            {
                yield return new Category()
                {
                    Classification = cls.Current,
                    Code = codes.Current,
                    Description = descs.Current
                };
            }
        }

        internal static IEnumerable<string> GetRequirementDescs(this CobieObject retType)
        {
            return GetStringListFromCompound(retType, RequirementDescsAttributeName);
        }

        internal static IEnumerable<string> GetRequirementClassifications(this CobieObject retType)
        {
            return GetStringListFromCompound(retType, RequirementCategoriesAttributeName);
        }

        internal static IEnumerable<string> GetRequirementCodes(this CobieObject retType)
        {
            return GetStringListFromCompound(retType, RequirementCodesAttributeName);
        }
        
        internal static IEnumerable<string> GetMatchingClassifications(this CobieObject retType)
        {
            return GetStringListFromCompound(retType, MatchingCategoriesAttributeName);
        }

        internal static IEnumerable<string> GetMatchingCodes(this CobieObject retType)
        {
            return GetStringListFromCompound(retType, MatchingCodesAttributeName);
        }


        private static IEnumerable<string> GetStringListFromCompound(CobieObject retType, string p)
        {
            if (retType.Attributes == null)
                return Enumerable.Empty<string>();

            var existingAttribute =
                retType.Attributes.FirstOrDefault(
                    a => a.Name == p && a.PropertySetName == AttributesPropertySetName);

            if (existingAttribute == null)
                return Enumerable.Empty<string>();

            var compoundStringValue = existingAttribute.Value as StringAttributeValue;
            return compoundStringValue == null
                ? Enumerable.Empty<string>()
                : compoundStringValue.Value.CompoundStringToList();
        }

        public static void SetMatchingCategories(this CobieObject retType, IEnumerable<Category> matchingCategories)
        {
            var categories = matchingCategories as Category[] ?? matchingCategories.ToArray();
            retType.SetMatchingClassifications(categories.Select(x=>x.Classification));
            retType.SetMatchingCodes(categories.Select(x => x.Code));
        }

        public static void SetRequirementCategories(this CobieObject retType, IEnumerable<Category> matchingCategories)
        {
            var categories = matchingCategories as Category[] ?? matchingCategories.ToArray();
            retType.SetRequirementClassifications(categories.Select(x => x.Classification));
            retType.SetRequirementCodes(categories.Select(x => x.Code));
            retType.SetRequirementDescs(categories.Select(x => x.Description));
        }

        private static void SetMatchingCodes(this CobieObject retType, IEnumerable<string> matchingCategories)
        {
            const string description = "Comma separated classification names of the validation candidate that match a requirement.";
            SetListToCompoundAttribute(retType, matchingCategories, MatchingCodesAttributeName, description);
        }

        private static void SetRequirementDescs(this CobieObject retType, IEnumerable<string> matchingCategories)
        {
            const string description = "Comma separated classification description applicable to the validation requirement.";
            SetListToCompoundAttribute(retType, matchingCategories, RequirementDescsAttributeName, description);
        }

        private static void SetRequirementCodes(this CobieObject retType, IEnumerable<string> matchingCategories)
        {
            const string description = "Comma separated classification codes applicable to the validation requirement.";
            SetListToCompoundAttribute(retType, matchingCategories, RequirementCodesAttributeName, description);
        }

        private static void SetMatchingClassifications(this CobieObject retType, IEnumerable<string> matchingCategories)
        {
            const string description = "Comma separated values of the validation candidate that match a classification requirement.";
            SetListToCompoundAttribute(retType, matchingCategories, MatchingCategoriesAttributeName, description);
        }

        private static void SetRequirementClassifications(this CobieObject retType, IEnumerable<string> matchingCategories)
        {
            const string description = "Comma separated classification names applicable to the validation requirement.";
            SetListToCompoundAttribute(retType, matchingCategories, RequirementCategoriesAttributeName, description);
        }

        private static void SetListToCompoundAttribute(CobieObject retType, IEnumerable<string> list, string propName,
            string description)
        {
            if (retType.Attributes == null)
                retType.Attributes = new List<Attribute>();

            var existingAttribute =
                retType.Attributes.FirstOrDefault(
                    a => a.Name == propName && a.PropertySetName == AttributesPropertySetName);

            var newValue = list.ListToCompoundString();

            if (existingAttribute != null)
            {
                existingAttribute.Value = new StringAttributeValue()
                {
                    Value = newValue
                };
            }
            else
            {
                var matchingClassAttribute = new Attribute
                {
                    Name = propName,
                    PropertySetName = AttributesPropertySetName,
                    Description = description,
                    Value = new StringAttributeValue() { Value = newValue },
                    Categories = new List<Category>() { dpowValidatedAttributeClass }
                };
                retType.Attributes.Add(matchingClassAttribute);
            }
        }
    }
}
