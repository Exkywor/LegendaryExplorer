﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ME3ExplorerCore.Packages;
using ME3ExplorerCore.Packages.CloningImportingAndRelinking;
using ME3ExplorerCore.Unreal;
using InterpCurveVector = ME3ExplorerCore.Unreal.BinaryConverters.InterpCurve<ME3ExplorerCore.SharpDX.Vector3>;
using InterpCurveFloat = ME3ExplorerCore.Unreal.BinaryConverters.InterpCurve<float>;

namespace ME3ExplorerCore.Matinee
{
    public static class MatineeHelper
    {
        public static ExportEntry AddNewGroupToInterpData(ExportEntry interpData, string groupName) => InternalAddGroup("InterpGroup", interpData, groupName);

        public static ExportEntry AddNewGroupDirectorToInterpData(ExportEntry interpData) => InternalAddGroup("InterpGroupDirector", interpData, null);

        private static ExportEntry InternalAddGroup(string className, ExportEntry interpData, string groupName)
        {
            var properties = new PropertyCollection{new ArrayProperty<ObjectProperty>("InterpTracks")};
            if (!string.IsNullOrEmpty(groupName))
            {
                properties.Add(new NameProperty(groupName, "GroupName"));
            }
            properties.Add(CommonStructs.ColorProp(className == "InterpGroup" ? Color.Green : Color.Purple, "GroupColor"));
            ExportEntry group = CreateNewExport(className, interpData, properties);

            var props = interpData.GetProperties();
            var groupsProp = props.GetProp<ArrayProperty<ObjectProperty>>("InterpGroups") ?? new ArrayProperty<ObjectProperty>("InterpGroups");
            groupsProp.Add(new ObjectProperty(group));
            props.AddOrReplaceProp(groupsProp);
            interpData.WriteProperties(props);

            return group;
        }

        private static ExportEntry CreateNewExport(string className, ExportEntry parent, PropertyCollection properties)
        {
            IMEPackage pcc = parent.FileRef;
            var group = new ExportEntry(pcc, properties: properties)
            {
                ObjectName = pcc.GetNextIndexedName(className),
                Class = EntryImporter.EnsureClassIsInFile(pcc, className)
            };
            group.ObjectFlags |= UnrealFlags.EObjectFlags.Transactional;
            pcc.AddExport(group);
            group.Parent = parent;
            return group;
        }

        public static List<ClassInfo> GetInterpTracks(MEGame game) => UnrealObjectInfo.GetNonAbstractDerivedClassesOf("InterpTrack", game);

        public static ExportEntry AddNewTrackToGroup(ExportEntry interpGroup, string trackClass)
        {
            //should add the property that contains track keys at least
            ExportEntry track = CreateNewExport(trackClass, interpGroup, null);

            var props = interpGroup.GetProperties();
            var tracksProp = props.GetProp<ArrayProperty<ObjectProperty>>("InterpTracks") ?? new ArrayProperty<ObjectProperty>("InterpTracks");
            tracksProp.Add(new ObjectProperty(track));
            props.AddOrReplaceProp(tracksProp);
            interpGroup.WriteProperties(props);

            return track;
        }

        public static void AddDefaultPropertiesToTrack(ExportEntry trackExport)
        {
            if (trackExport.IsA("BioInterpTrack"))
            {
                if (trackExport.IsA("SFXInterpTrackToggleBase"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aToggleKeyData"));
                }
                else if (trackExport.IsA("BioConvNodeTrackDebug"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StrProperty>("m_aDbgStrings"));
                }
                else if (trackExport.IsA("BioEvtSysTrackDOF"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aDOFData"));
                }
                else if (trackExport.IsA("BioEvtSysTrackGesture"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aGestures"));
                }
                else if (trackExport.IsA("BioEvtSysTrackInterrupt"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aInterruptData"));
                }
                else if (trackExport.IsA("BioEvtSysTrackLighting"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aLightingKeys"));
                }
                else if (trackExport.IsA("BioEvtSysTrackLookAt"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aLookAtKeys"));
                }
                else if (trackExport.IsA("BioEvtSysTrackProp"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aPropKeys"));
                }
                else if (trackExport.IsA("BioEvtSysTrackSetFacing"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aFacingKeys"));
                }
                else if (trackExport.IsA("BioEvtSysTrackSubtitles"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aSubtitleData"));
                }
                else if (trackExport.IsA("BioEvtSysTrackSwitchCamera"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aCameras"));
                }
                else if (trackExport.IsA("BioInterpTrackRotationMode"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("EventTrack"));
                }
                else if (trackExport.IsA("SFXGameInterpTrackProcFoley"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aProcFoleyStartStopKeys"));
                    trackExport.WriteProperty(new ObjectProperty(0, "m_TrackFoleySound"));
                }
                else if (trackExport.IsA("SFXGameInterpTrackWwiseMicLock"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aMicLockKeys"));
                }
                else if (trackExport.IsA("SFXInterpTrackAttachCrustEffect"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aCrustEffectKeyData"));
                    trackExport.WriteProperty(new ObjectProperty(0, "oEffect"));
                }
                else if (trackExport.IsA("SFXInterpTrackBlackScreen"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aBlackScreenKeyData"));
                }
                else if (trackExport.IsA("SFXInterpTrackLightEnvQuality"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aLightEnvKeyData"));
                }
                else if (trackExport.IsA("SFXInterpTrackMovieBase"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aMovieKeyData"));
                }
                else if (trackExport.IsA("SFXInterpTrackSetPlayerNearClipPlane"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aNearClipKeyData"));
                }
                else if (trackExport.IsA("SFXInterpTrackSetWeaponInstant"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aWeaponClassKeyData"));
                }
                else if (trackExport.IsA("SFXInterpTrackPlayFaceOnlyVO"))
                {
                    trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aFOVOKeys"));
                }

                trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aTrackKeys"));
            }
            else if (trackExport.ClassName == "InterpTrackSound")
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("Sounds"));
                trackExport.WriteProperty(new InterpCurveVector().ToStructProperty(trackExport.Game, "VectorTrack"));
            }
            else if (trackExport.IsA("InterpTrackEvent"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("EventTrack"));
            }
            else if (trackExport.IsA("InterpTrackFaceFX"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("FaceFXSeqs"));
            }
            else if (trackExport.IsA("InterpTrackAnimControl"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("AnimSeqs"));
            }
            else if (trackExport.IsA("InterpTrackMove"))
            {
                trackExport.WriteProperty(new InterpCurveVector().ToStructProperty(trackExport.Game, "PosTrack"));
                trackExport.WriteProperty(new InterpCurveVector().ToStructProperty(trackExport.Game, "EulerTrack"));
                trackExport.WriteProperty(new StructProperty("InterpLookupTrack", new PropertyCollection
                {
                    new ArrayProperty<StructProperty>("Points")
                }, "LookupTrack"));
            }
            else if (trackExport.IsA("InterpTrackVisibility"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("VisibilityTrack"));
            }
            else if (trackExport.IsA("InterpTrackToggle"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("ToggleTrack"));
            }
            else if (trackExport.IsA("InterpTrackWwiseEvent"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("WwiseEvents"));
            }
            else if (trackExport.IsA("InterpTrackDirector"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("CutTrack"));
            }
            else if (trackExport.IsA("BioEvtSysTrackSubtitles"))
            {
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aSubtitleData"));
                trackExport.WriteProperty(new ArrayProperty<StructProperty>("m_aTrackKeys"));
            }
            else if (trackExport.IsA("InterpTrackFloatBase"))
            {
                trackExport.WriteProperty(new InterpCurveFloat().ToStructProperty(trackExport.Game, "FloatTrack"));
            }
            else if (trackExport.IsA("InterpTrackVectorBase"))
            {
                trackExport.WriteProperty(new InterpCurveVector().ToStructProperty(trackExport.Game, "VectorTrack"));
            }
        }
    }
}
