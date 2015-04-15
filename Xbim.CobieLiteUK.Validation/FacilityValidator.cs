﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xbim.COBieLiteUK;

namespace Xbim.CobieLiteUK.Validation
{
    public class FacilityValidator : IValidator
    {
        public FacilityValidator()
        {
            TerminationMode = TerminationMode.ExecuteCompletely;
        }

        internal static readonly Category FailedCat = new Category()
        {
            Classification = "DPoW",
            Code = "Failed"
        };

        internal static readonly Category PassedCat = new Category()
        {
            Classification = "DPoW",
            Code = "Passed"
        };

        public Facility Validate(Facility requirement, Facility submitted)
        {
            var retFacility = new Facility {Categories = new List<Category>()};
            var sb = new StringBuilder();

            // a facility validation passes is carried out through the validation of
            // a) local values
            // b) Project
            // c) assetTypes (WIP)
            // d) spaces (planned)

            // a)
            bool facilityPasses = true;

            // area units
            if (requirement.AreaUnitsCustom != submitted.AreaUnitsCustom)
            {
                retFacility.AreaUnitsCustom = string.Format("{0} (should be '{1}')", submitted.AreaUnitsCustom, requirement.AreaUnitsCustom);
                sb.AppendFormat("{0} failure: {1}\r\n", "Area units", retFacility.AreaUnitsCustom);
                facilityPasses = false;
            }
            else
            {
                retFacility.AreaUnitsCustom = submitted.AreaUnitsCustom;
            }

            // linear units
            if (requirement.LinearUnitsCustom != submitted.LinearUnitsCustom)
            {
                retFacility.LinearUnitsCustom = string.Format("{0} (should be '{1}')", submitted.LinearUnitsCustom, requirement.LinearUnitsCustom);
                sb.AppendFormat("{0} failure: {1}\r\n", "Linear units", retFacility.LinearUnitsCustom);
                facilityPasses = false;
            }
            else
            {
                retFacility.LinearUnitsCustom = submitted.LinearUnitsCustom;
            }

            // Volume units
            if (requirement.VolumeUnitsCustom != submitted.VolumeUnitsCustom)
            {
                retFacility.VolumeUnitsCustom = string.Format("{0} (should be '{1}')", submitted.VolumeUnitsCustom, requirement.VolumeUnitsCustom);
                sb.AppendFormat("{0} failure: {1}\r\n", "Volume units", retFacility.VolumeUnitsCustom);
                facilityPasses = false;
            }
            else
            {
                retFacility.VolumeUnitsCustom = submitted.VolumeUnitsCustom;
            }

            // Currency units
            if (requirement.CurrencyUnitCustom != submitted.CurrencyUnitCustom)
            {
                retFacility.CurrencyUnitCustom = string.Format("{0} (should be '{1}')", submitted.CurrencyUnitCustom, requirement.CurrencyUnitCustom);
                sb.AppendFormat("{0} failure: {1}\r\n", "Currency units", retFacility.CurrencyUnitCustom);
                facilityPasses = false;
            }
            else
            {
                retFacility.CurrencyUnitCustom = submitted.CurrencyUnitCustom;
            }

            if (requirement.Project != null)
            {
                // to be added project level validation here.
                var pv = new ProjectValidator(requirement.Project);
                retFacility.Project = pv.Validate(submitted.Project);

                if (!pv.IsPass)
                {
                    sb.AppendFormat("Validation of Project information fails, see project information for detail.\r\n");
                    facilityPasses = false;
                }
            }
            // c) asset types
            if (requirement.AssetTypes != null)
            {
                foreach (var assetTypeRequirement in requirement.AssetTypes)
                {
                    var v = new CobieObjectValidator<AssetType, Asset>(assetTypeRequirement)
                    {
                        TerminationMode = TerminationMode
                    };
                    if (! v.HasRequirements)
                        continue;
                    var candidates = v.GetCandidates(submitted.AssetTypes).ToList();
                    // ReSharper disable once PossibleMultipleEnumeration
                    if (candidates.Any())
                    {
                        foreach (var candidate in candidates)
                        {
                            if (retFacility.AssetTypes == null)
                                retFacility.AssetTypes = new List<AssetType>();
                            retFacility.AssetTypes.Add(v.Validate(candidate, retFacility));
                        }
                    }
                    else
                    {
                        if (retFacility.AssetTypes == null)
                            retFacility.AssetTypes = new List<AssetType>();
                        retFacility.AssetTypes.Add(v.Validate((AssetType)null, retFacility));
                    }
                }
            }
            // d) zones
            if (requirement.Zones != null)
            {
                // hack: create a fake modelFacility candidates from spaces.
                var fakeSubmittedFacility = new Facility();
                fakeSubmittedFacility.Floors = fakeSubmittedFacility.Clone(submitted.Floors as IEnumerable<Floor>).ToList();

                foreach (var zoneRequirement in requirement.Zones)
                {
                    var v = new CobieObjectValidator<Zone, Space>(zoneRequirement)
                    {
                        TerminationMode = TerminationMode
                    };
                    if (! v.HasRequirements)
                        continue;
                    // var candidates = v.GetCandidates(submitted.Zones).ToList();
                    // hack: now create a fake Zone based on candidates from spaces.
                    var lSpaces = submitted.Get<Space>().ToList();
                    var candidateSpaces = v.GetCandidates(lSpaces);

                    foreach (var spaceMatch in candidateSpaces)
                    {
                        Debug.WriteLine(spaceMatch);
                    }


                    //// ReSharper disable once PossibleMultipleEnumeration
                    //if (candidates.Any())
                    //{
                    //    foreach (var candidate in candidates)
                    //    {
                    //        if (retFacility.Zones == null)
                    //            retFacility.Zones = new List<Zone>();
                    //        retFacility.Zones.Add(v.Validate(candidate, retFacility));
                    //    }
                    //}
                    //else
                    //{
                    //    if (retFacility.Zones == null)
                    //        retFacility.Zones = new List<Zone>();
                    //    retFacility.Zones.Add(v.Validate((Zone)null, retFacility));
                    //}
                }
            }
            retFacility.Description = sb.ToString();
            retFacility.Categories.Add(facilityPasses ? PassedCat : FailedCat);

            return retFacility;
        }

        public TerminationMode TerminationMode { get; set; }
        public bool HasFailures { get; set; }
    }
}
