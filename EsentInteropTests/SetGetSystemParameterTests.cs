﻿//-----------------------------------------------------------------------
// <copyright file="SetGetSystemParameterTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

namespace InteropApiTests
{
    using System;
    using System.IO;
    using Microsoft.Isam.Esent.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Jet{Get,Set}SystemParameter tests
    /// </summary>
    [TestClass]
    public class SetGetSystemParameterTests
    {
        /// <summary>
        /// Test setting and retrieving the system path.
        /// </summary>
        [TestMethod]
        public void SystemPathParameter()
        {
            this.PathParameterTest(JET_param.SystemPath, @"foo\system\");
        }

        /// <summary>
        /// Test setting and retrieving the log path.
        /// </summary>
        [TestMethod]
        public void LogPathParameter()
        {
            this.PathParameterTest(JET_param.LogFilePath, @"foo\log\");
        }

        /// <summary>
        /// Test setting and retrieving the temp path.
        /// </summary>
        [TestMethod]
        public void TempPathParameter()
        {
            this.PathParameterTest(JET_param.TempPath, @"foo\temp\");
        }

        /// <summary>
        /// Test setting and retrieving the base name.
        /// </summary>
        [TestMethod]
        public void BaseNameParameter()
        {
            this.StringParameterTest(JET_param.BaseName, "foo");
        }

        /// <summary>
        /// Test setting and retrieving the event source.
        /// </summary>
        [TestMethod]
        public void EventSourceParameter()
        {
            this.StringParameterTest(JET_param.EventSource, "My source");
        }

        /// <summary>
        /// Test setting and retrieving the max sessions setting.
        /// </summary>
        [TestMethod]
        public void MaxSessionsParameter()
        {
            this.IntegerParameterTest(JET_param.MaxSessions, 4);
        }

        /// <summary>
        /// Test setting and retrieving the max open tables setting.
        /// </summary>
        [TestMethod]
        public void MaxOpenTablesParameter()
        {
            this.IntegerParameterTest(JET_param.MaxOpenTables, 100);
        }

        /// <summary>
        /// Test setting and retrieving the max cursors setting.
        /// </summary>
        [TestMethod]
        public void MaxCursorsParameter()
        {
            this.IntegerParameterTest(JET_param.MaxCursors, 2500);
        }

        /// <summary>
        /// Test setting and retrieving the max ver pages setting.
        /// </summary>
        [TestMethod]
        public void MaxVerPagesParameter()
        {
            this.IntegerParameterTest(JET_param.MaxVerPages, 100);
        }

        /// <summary>
        /// Test setting and retrieving the max temporary tables setting.
        /// </summary>
        [TestMethod]
        public void MaxTemporaryTablesParameter()
        {
            this.IntegerParameterTest(JET_param.MaxTemporaryTables, 0);
        }

        /// <summary>
        /// Test setting and retrieving the logfile size setting.
        /// </summary>
        [TestMethod]
        public void LogFileSizeParameter()
        {
            this.IntegerParameterTest(JET_param.LogFileSize, 2048);
        }

        /// <summary>
        /// Test setting and retrieving the circular logging setting.
        /// </summary>
        [TestMethod]
        public void CircularLogParameter()
        {
            this.IntegerParameterTest(JET_param.CircularLog, 1);
        }

        /// <summary>
        /// Test setting and retrieving the checkpoint depth setting.
        /// </summary>
        [TestMethod]
        public void CheckpointDepthMaxParameter()
        {
            this.IntegerParameterTest(JET_param.CheckpointDepthMax, 20000);
        }

        /// <summary>
        /// Test setting and retrieving the recovery parameter.
        /// </summary>
        [TestMethod]
        public void RecoveryParameter()
        {
            this.StringParameterTest(JET_param.Recovery, "off");
        }

        /// <summary>
        /// Test setting and retrieving the index checking setting.
        /// </summary>
        [TestMethod]
        public void EnableIndexCheckingParameter()
        {
            this.IntegerParameterTest(JET_param.EnableIndexChecking, 1);
        }

        /// <summary>
        /// Test setting and retrieving the no information event setting.
        /// </summary>
        [TestMethod]
        public void NoInformationEventParameter()
        {
            this.IntegerParameterTest(JET_param.NoInformationEvent, 1);
        }

        /// <summary>
        /// Test setting and retrieving the create path setting.
        /// </summary>
        [TestMethod]
        public void CreatePathIfNotExistParameter()
        {
            this.IntegerParameterTest(JET_param.CreatePathIfNotExist, 1);
        }

        /// <summary>
        /// Test setting and retrieving a system parameter that uses a path. A relative
        /// path is set but a full path is retrieved.
        /// </summary>
        /// <param name="param">The parameter to set.</param>
        /// <param name="expected">The path to set it to.</param>
        private void PathParameterTest(JET_param param, string expected)
        {
            JET_INSTANCE instance;
            Api.JetCreateInstance(out instance, "PathParameterTest");
            try
            {
                Api.JetSetSystemParameter(instance, JET_SESID.Nil, param, 0, expected);

                int ignored = 0;
                string actual;
                Api.JetGetSystemParameter(instance, JET_SESID.Nil, param, ref ignored, out actual, 256);

                Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, expected), actual);
            }
            finally
            {
                Api.JetTerm(instance);
            }
        }

        /// <summary>
        /// Test setting and retrieving a system parameter that uses a string.
        /// </summary>
        /// <param name="param">The parameter to set.</param>
        /// <param name="expected">The string to set it to.</param>
        private void StringParameterTest(JET_param param, string expected)
        {
            JET_INSTANCE instance;
            Api.JetCreateInstance(out instance, "StringParameterTest");
            try
            {
                Api.JetSetSystemParameter(instance, JET_SESID.Nil, param, 0, expected);

                int ignored = 0;
                string actual;
                Api.JetGetSystemParameter(instance, JET_SESID.Nil, param, ref ignored, out actual, 256);

                Assert.AreEqual(expected, actual);
            }
            finally
            {
                Api.JetTerm(instance);
            }
        }

        /// <summary>
        /// Test setting and retrieving an integer system parameter..
        /// </summary>
        /// <param name="param">The parameter to set.</param>
        /// <param name="expected">The string to set it to.</param>
        private void IntegerParameterTest(JET_param param, int expected)
        {
            JET_INSTANCE instance;
            Api.JetCreateInstance(out instance, "IntParameterTest");
            try
            {
                Api.JetSetSystemParameter(instance, JET_SESID.Nil, param, expected, null);

                int actual = 0;
                string ignored;
                Api.JetGetSystemParameter(instance, JET_SESID.Nil, param, ref actual, out ignored, 0);

                Assert.AreEqual(expected, actual);
            }
            finally
            {
                Api.JetTerm(instance);
            }
        }
    }
}