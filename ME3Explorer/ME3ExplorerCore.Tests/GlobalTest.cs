﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ME3ExplorerCore.GameFilesystem;
using ME3ExplorerCore.Packages;
using ME3ExplorerCore.Unreal;

namespace ME3ExplorerCore.Tests
{
    public static class GlobalTest
    {
        private static bool initialized;

        public static void Init()
        {
            if (initialized) return;
            var sc = new SynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(sc);
            ME3ExplorerCoreLib.InitLib(TaskScheduler.FromCurrentSynchronizationContext(), x => { });
#if AZURE
            ME1Directory.DefaultGamePath = ME1UnrealObjectInfo.MiniGameFilesPath = GetTestMiniGamePath(MEGame.ME1);
            ME2Directory.DefaultGamePath = ME2UnrealObjectInfo.MiniGameFilesPath = GetTestMiniGamePath(MEGame.ME2);
            ME3Directory.DefaultGamePath = ME3UnrealObjectInfo.MiniGameFilesPath = GetTestMiniGamePath(MEGame.ME3);
#endif
            initialized = true;
        }
        /// <summary>
        /// Looks in parent folders for folder containing a folder named "testdata" as Azure DevOps seems to build project differently than on a VS installation
        /// </summary>
        /// <returns></returns>
        public static string GetTestDataDirectory()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            while (Directory.GetParent(dir.FullName) != null)
            {
                dir = Directory.GetParent(dir.FullName);
                var testDataPath = Path.Combine(dir.FullName, "testdata");
                if (Directory.Exists(testDataPath)) return testDataPath;
            }

            throw new Exception("Could not find testdata directory!");
        }

        public static string GetTestMiniGamePath(MEGame game) => Path.Combine(GetTestDataDirectory(), "dynamiclookupminigame", game.ToString());

        public static string GetTestPackagesDirectory() => Path.Combine(GetTestDataDirectory(), "packages");
        public static string GetTestSFARsDirectory() => Path.Combine(GetTestDataDirectory(), "sfars");
        public static string GetTestMountsDirectory() => Path.Combine(GetTestDataDirectory(), "mounts");
        public static string GetTestTLKDirectory() => Path.Combine(GetTestDataDirectory(), "tlk");
        public static string GetTestDataMiscDirectory() => Path.Combine(GetTestDataDirectory(), "misc");
        public static string GetTestCRCDirectory() => Path.Combine(GetTestDataMiscDirectory(), "crc");
        public static string GetTestTexturesDirectory() => Path.Combine(GetTestDataDirectory(), "textures");


        public static (MEGame expectedGame, MEPackage.GamePlatform expectedPlatform) GetExpectedTypes(string p)
        {
            MEPackage.GamePlatform expectedPlatform = MEPackage.GamePlatform.Unknown;
            MEGame expectedGame = MEGame.Unknown;

            string parentname = Directory.GetParent(p).FullName;
            int level = 0;
            while (parentname != null)
            {
                var dirname = Path.GetFileName(parentname);
                if (dirname == "retail" || dirname == "demo")
                {
                    parentname = Directory.GetParent(parentname).FullName;
                    continue;
                }

                if (level == 0)
                {
                    expectedGame = Enum.Parse<MEGame>(dirname);
                }
                else if (level == 1)
                {
                    expectedPlatform = Enum.Parse<MEPackage.GamePlatform>(dirname);
                }
                else
                {
                    break;
                }

                parentname = Directory.GetParent(parentname).FullName;
                level++;
            }

            return (expectedGame, expectedPlatform);
        }
    }
}
